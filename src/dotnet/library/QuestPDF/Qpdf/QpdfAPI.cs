using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using QuestPDF.Skia;

namespace QuestPDF.Qpdf;

static class QpdfAPI
{
    /// <summary>
    /// The job configuration references in-memory buffers with this prefix in place of file paths.
    /// </summary>
    public const string BufferReferencePrefix = "qpdf-buffer://";

    private const string OutputBufferName = "output";
    public const string OutputBufferReference = BufferReferencePrefix + OutputBufferName;

    public static int GetCompatibilityVersion()
    {
        return API.get_questpdf_version();
    }
    
    public static void ExecuteJob(
        string jobJson,
        IReadOnlyDictionary<string, byte[]>? inputBuffers = null,
        Stream? outputStream = null,
        Func<byte[], byte[]>? transformMetadata = null)
    {
        QpdfNativeDependencyCompatibilityChecker.Test();
        
        // create StringBuilder that will store the error message
        var error = new StringBuilder();
        var errorHandle = GCHandle.Alloc(error);
        var errorPtr = GCHandle.ToIntPtr(errorHandle);

        var logger = IntPtr.Zero;
        var jobHandle = IntPtr.Zero;
        var documentHandle = IntPtr.Zero;

        var inputHandles = new List<GCHandle>();
        var output = outputStream != null ? new OutputStreamWriter(outputStream) : null;
        var outputHandle = default(GCHandle);

        try
        {
            // create logger
            logger = API.qpdflogger_create();
            API.qpdflogger_set_error(logger, 4, LoggingCallbackPointer, errorPtr); // 4 = custom logger

            // prepare the job
            jobHandle = API.qpdfjob_init();
            API.qpdfjob_set_logger(jobHandle, logger);
            RegisterInputBuffers();
            RegisterOutputBuffer();
            ThrowOnJobError(API.qpdfjob_initialize_from_json(jobHandle, jobJson));

            // load the document and apply all operations
            documentHandle = API.qpdfjob_create_qpdf(jobHandle);

            if (documentHandle == IntPtr.Zero)
                ThrowOnJobError(JobResultError);

            if (transformMetadata != null)
                TransformMetadata(transformMetadata);

            // write the output file
            ThrowOnJobError(API.qpdfjob_write_qpdf(jobHandle, documentHandle));
        }
        finally
        {
            if (documentHandle != IntPtr.Zero)
                API.qpdf_cleanup(ref documentHandle);

            if (jobHandle != IntPtr.Zero)
                API.qpdfjob_cleanup(ref jobHandle);

            if (logger != IntPtr.Zero)
                API.qpdflogger_cleanup(ref logger);

            // qpdf reads the input buffers lazily and keeps pointers to them until the clean-up above
            foreach (var inputHandle in inputHandles)
                inputHandle.Free();

            if (outputHandle.IsAllocated)
                outputHandle.Free();

            errorHandle.Free();
        }

        void RegisterInputBuffers()
        {
            if (inputBuffers == null)
                return;

            foreach (var inputBuffer in inputBuffers)
            {
                // qpdf reads the buffers in place, without copying them
                var inputHandle = GCHandle.Alloc(inputBuffer.Value, GCHandleType.Pinned);
                inputHandles.Add(inputHandle);

                ThrowOnJobError(API.qpdfjob_register_buffer_input(jobHandle, inputBuffer.Key, inputHandle.AddrOfPinnedObject(), (UIntPtr)inputBuffer.Value.Length));
            }
        }

        void RegisterOutputBuffer()
        {
            if (output == null)
                return;

            outputHandle = GCHandle.Alloc(output);
            ThrowOnJobError(API.qpdfjob_register_buffer_output(jobHandle, OutputBufferName, OutputCallbackPointer, GCHandle.ToIntPtr(outputHandle)));
        }

        void TransformMetadata(Func<byte[], byte[]> transform)
        {
            ThrowOnJobError(API.questpdf_job_get_xmp_metadata(jobHandle, documentHandle, out var buffer, out var length));

            if (buffer == IntPtr.Zero)
            {
                throw new Exception(
                    "QuestPDF could not extend the document metadata because the document does not contain any XMP metadata. " +
                    "If the document is generated with QuestPDF, please enable the PDF/A or PDF/UA conformance in the DocumentSettings.");
            }

            byte[] content;

            try
            {
                content = new byte[(int)length];
                Marshal.Copy(buffer, content, 0, content.Length);
            }
            finally
            {
                API.qpdf_oh_free_buffer(ref buffer);
            }

            content = transform(content);

            // qpdf copies the content, so the managed array is needed only for the duration of the call
            ThrowOnJobError(API.questpdf_job_set_xmp_metadata(jobHandle, documentHandle, content, (UIntPtr)content.Length));
        }

        void ThrowOnJobError(int jobResultId)
        {
            // when the output stream fails, qpdf only reports that the job was aborted, so rethrow the root cause instead
            output?.WriteException?.Throw();

            // 0 = success, 1 = undefined, 2 = error, 3 = warning
            if (jobResultId == JobResultError)
                throw new Exception($"QuestPDF could not perform document operation:\n\n{error}");
        }
    }
    
    private const int JobResultError = 2;

    #region Output Stream

    private sealed class OutputStreamWriter(Stream targetStream)
    {
        public ExceptionDispatchInfo? WriteException { get; private set; }

        public int Write(IntPtr data, UIntPtr length)
        {
            try
            {
                var size = checked((int)length.ToUInt64());

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                unsafe
                {
                    targetStream.Write(new ReadOnlySpan<byte>((void*)data, size));
                }
#else
                var managedArray = new byte[size];
                Marshal.Copy(data, managedArray, 0, size);
                targetStream.Write(managedArray, 0, size);
#endif

                return 0;
            }
            catch (Exception exception)
            {
                // exceptions cannot cross the native boundary, so they are captured and rethrown after the job is aborted
                WriteException = ExceptionDispatchInfo.Capture(exception);
                return 1; // any non-zero value aborts the job
            }
        }
    }

    private static int OutputCallback(IntPtr data, UIntPtr length, IntPtr udata)
    {
        var handle = GCHandle.FromIntPtr(udata);
        var writer = (OutputStreamWriter)handle.Target!;
        return writer.Write(data, length);
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int OutputCallbackDelegate(IntPtr data, UIntPtr length, IntPtr udata);

    private static readonly OutputCallbackDelegate OutputCallbackDelegateInstance = OutputCallback;

    private static readonly IntPtr OutputCallbackPointer = Marshal.GetFunctionPointerForDelegate(OutputCallbackDelegateInstance);

    #endregion

    #region Logging
    
    private static int LoggingCallback(IntPtr data, int length, IntPtr udata)
    {
        var bytes = new byte[length];
        Marshal.Copy(data, bytes, 0, length);

        var handle = GCHandle.FromIntPtr(udata);
        var stringBuilder = (StringBuilder)handle.Target;
        stringBuilder?.Append(Encoding.UTF8.GetString(bytes));

        return 0;
    }
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int CallbackDelegate(IntPtr data, int length, IntPtr udata);
    
    private static readonly CallbackDelegate LoggingCallbackDelegate = LoggingCallback;
    
    private static readonly IntPtr LoggingCallbackPointer = Marshal.GetFunctionPointerForDelegate(LoggingCallbackDelegate);
    
    #endregion
    
    private static class API
    {
        const string LibraryName = "qpdf";
        
        /* GENERAL */
        
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_questpdf_version();
    
        /* JOBS */
        
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr qpdfjob_init();
    
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void qpdfjob_cleanup(ref IntPtr jobHandle);
    
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int qpdfjob_initialize_from_json(IntPtr jobHandle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8StringMarshaller))] string json);

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr qpdfjob_create_qpdf(IntPtr jobHandle);

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int qpdfjob_write_qpdf(IntPtr jobHandle, IntPtr documentHandle);

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void qpdf_cleanup(ref IntPtr documentHandle);

        /* METADATA */

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int questpdf_job_get_xmp_metadata(IntPtr jobHandle, IntPtr documentHandle, out IntPtr buffer, out UIntPtr length);

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int questpdf_job_set_xmp_metadata(IntPtr jobHandle, IntPtr documentHandle, byte[] content, UIntPtr length);

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void qpdf_oh_free_buffer(ref IntPtr buffer);
        
        /* IN-MEMORY BUFFERS */

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int qpdfjob_register_buffer_input(IntPtr jobHandle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8StringMarshaller))] string name, IntPtr data, UIntPtr length);

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int qpdfjob_register_buffer_output(IntPtr jobHandle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8StringMarshaller))] string name, IntPtr callback, IntPtr udata);

        /* LOGGING */
        
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = nameof(qpdflogger_create))]
        public static extern IntPtr qpdflogger_create();

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = nameof(qpdflogger_cleanup))]
        public static extern void qpdflogger_cleanup(ref IntPtr loggerHandle);
        
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = nameof(qpdflogger_set_error))]
        public static extern void qpdflogger_set_error(IntPtr loggerHandle, int destination, IntPtr callBackHandler, IntPtr udata);
        
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = nameof(qpdfjob_set_logger))]
        public static extern void qpdfjob_set_logger(IntPtr jobHandle, IntPtr loggerHandle);
    }
}