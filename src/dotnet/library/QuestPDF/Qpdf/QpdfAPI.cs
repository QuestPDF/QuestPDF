using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using QuestPDF.Skia;

namespace QuestPDF.Qpdf;

static class QpdfAPI
{
    /// <summary>
    /// Job JSON references registered in-memory buffers using this scheme in place of file names,
    /// e.g. "inputFile": "qpdf-buffer://main".
    /// </summary>
    public const string BufferReferenceScheme = "qpdf-buffer://";

    public static int GetCompatibilityVersion()
    {
        return API.questpdf_get_compatibility_version();
    }

    public static void ExecuteJob(string jobJson)
    {
        ExecuteJob(jobJson, inputBuffers: null, outputBufferName: null, outputStream: null);
    }

    /// <summary>
    /// Executes a qpdf job and returns the produced document. Documents passed via <paramref name="inputBuffers"/>
    /// can be referenced in the job JSON as "qpdf-buffer://name" anywhere an input file name is accepted.
    /// The job JSON must set "outputFile": "qpdf-buffer://name" matching <paramref name="outputBufferName"/>.
    /// </summary>
    public static byte[] ExecuteJob(string jobJson, IReadOnlyDictionary<string, byte[]>? inputBuffers, string outputBufferName)
    {
        using var output = new MemoryStream();
        ExecuteJob(jobJson, inputBuffers, outputBufferName, output);
        return output.ToArray();
    }

    /// <summary>
    /// Executes a qpdf job, streaming the produced document into <paramref name="outputStream"/> chunk by chunk,
    /// without materializing it in memory. The stream is written synchronously on the calling thread while the job runs.
    /// When <paramref name="outputBufferName"/> is null, the job JSON must set "outputFile" to a file path instead.
    /// </summary>
    public static void ExecuteJob(string jobJson, IReadOnlyDictionary<string, byte[]>? inputBuffers, string? outputBufferName, Stream? outputStream)
    {
        if ((outputBufferName == null) != (outputStream == null))
            throw new ArgumentException("The outputBufferName and outputStream arguments must be provided together.");

        QpdfNativeDependencyCompatibilityChecker.Test();

        // create StringBuilder that will store the error message
        var error = new StringBuilder();

        // receives the output document chunks, if requested; also captures any exception thrown
        // by the destination stream so it is not propagated through native frames
        var output = outputStream != null ? new OutputContext(outputStream) : null;

        // input buffers must stay pinned until the job completes: the native side reads them
        // directly, without copying
        var pinnedInputs = new List<GCHandle>();

        // every handle below is allocated inside the try block, so that the finally block
        // releases it even when a subsequent allocation fails
        var errorHandle = default(GCHandle);
        var outputHandle = default(GCHandle);
        var logger = IntPtr.Zero;
        var jobHandle = IntPtr.Zero;

        try
        {
            errorHandle = GCHandle.Alloc(error);
            var errorPtr = GCHandle.ToIntPtr(errorHandle);

            if (output != null)
                outputHandle = GCHandle.Alloc(output);

            // create logger and job
            logger = API.qpdflogger_create();
            jobHandle = API.qpdfjob_init();

            API.qpdflogger_set_error(logger, 4, LoggingCallbackPointer, errorPtr); // 4 = custom logger
            API.qpdfjob_set_logger(jobHandle, logger);

            if (inputBuffers != null)
            {
                foreach (var inputBuffer in inputBuffers)
                {
                    var pin = GCHandle.Alloc(inputBuffer.Value, GCHandleType.Pinned);
                    pinnedInputs.Add(pin);

                    var registerResultId = API.qpdfjob_register_buffer_input(
                        jobHandle, inputBuffer.Key, pin.AddrOfPinnedObject(), (nuint)inputBuffer.Value.Length);

                    if (registerResultId != 0)
                        throw CreateJobException(error);
                }
            }

            if (output != null)
            {
                var registerResultId = API.qpdfjob_register_buffer_output(
                    jobHandle, outputBufferName!, OutputCallbackPointer, GCHandle.ToIntPtr(outputHandle));

                if (registerResultId != 0)
                    throw CreateJobException(error);
            }

            // perform the job
            var jobResultId = API.qpdfjob_initialize_from_json(jobHandle, jobJson);

            if (jobResultId == 0)
                jobResultId = API.qpdfjob_run(jobHandle);

            // check errors: a destination stream failure takes precedence because it is the
            // root cause of the job failure that follows it
            if (output?.Exception != null)
                throw new Exception("QuestPDF could not write the output document to the provided stream.", output.Exception);

            var isError = jobResultId is 2; // 0 = success, 1 = undefined, 2 = error, 3 = warning

            if (isError)
                throw CreateJobException(error);
        }
        finally
        {
            if (jobHandle != IntPtr.Zero)
                API.qpdfjob_cleanup(ref jobHandle);

            if (logger != IntPtr.Zero)
                API.qpdflogger_cleanup(ref logger);

            foreach (var pin in pinnedInputs)
                pin.Free();

            if (outputHandle.IsAllocated)
                outputHandle.Free();

            if (errorHandle.IsAllocated)
                errorHandle.Free();
        }
    }

    private static Exception CreateJobException(StringBuilder error)
    {
        return new Exception($"QuestPDF could not perform document operation:\n\n{error}");
    }

    #region Logging

    private static int LoggingCallback(IntPtr data, int length, IntPtr udata)
    {
        var bytes = new byte[length];
        Marshal.Copy(data, bytes, 0, length);

        var handle = GCHandle.FromIntPtr(udata);
        var stringBuilder = (StringBuilder?)handle.Target;
        stringBuilder?.Append(Encoding.UTF8.GetString(bytes));

        return 0;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int CallbackDelegate(IntPtr data, int length, IntPtr udata);

    private static readonly CallbackDelegate LoggingCallbackDelegate = LoggingCallback;

    private static readonly IntPtr LoggingCallbackPointer = Marshal.GetFunctionPointerForDelegate(LoggingCallbackDelegate);

    #endregion

    #region Buffer Output

    private sealed class OutputContext
    {
        public Stream Stream { get; }
        public Exception? Exception { get; set; }

#if !NET5_0_OR_GREATER
        /// <summary>
        /// Reused across chunks: qpdf produces the document in many small pieces
        /// (thousands of them per megabyte), so allocating one array per chunk would
        /// dominate the cost of the entire operation.
        /// </summary>
        public byte[]? Buffer { get; set; }
#endif

        public OutputContext(Stream stream)
        {
            Stream = stream;
        }
    }

    private static int OutputCallback(IntPtr data, nuint length, IntPtr udata)
    {
        var handle = GCHandle.FromIntPtr(udata);
        var context = (OutputContext?)handle.Target;

        if (context == null)
            return 1;

        // exceptions must not unwind through native frames: capture them and abort the job by
        // returning a non-zero value
        try
        {
            var size = checked((int)length);

#if NET5_0_OR_GREATER
            // the native memory stays valid for the duration of the callback, and the write is
            // synchronous, so the chunk can be passed to the stream without copying it first
            unsafe
            {
                context.Stream.Write(new ReadOnlySpan<byte>((void*)data, size));
            }
#else
            if (context.Buffer == null || context.Buffer.Length < size)
                context.Buffer = new byte[size];

            Marshal.Copy(data, context.Buffer, 0, size);
            context.Stream.Write(context.Buffer, 0, size);
#endif

            return 0;
        }
        catch (Exception exception)
        {
            context.Exception = exception;
            return 1;
        }
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int OutputCallbackDelegate(IntPtr data, nuint length, IntPtr udata);

    private static readonly OutputCallbackDelegate OutputCallbackDelegateInstance = OutputCallback;

    private static readonly IntPtr OutputCallbackPointer = Marshal.GetFunctionPointerForDelegate(OutputCallbackDelegateInstance);

    #endregion

    private static class API
    {
        const string LibraryName = "qpdf";

        /* GENERAL */

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int questpdf_get_compatibility_version();

        /* JOBS */

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr qpdfjob_init();

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void qpdfjob_cleanup(ref IntPtr jobHandle);

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int qpdfjob_initialize_from_json(IntPtr jobHandle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8StringMarshaller))] string json);

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int qpdfjob_run(IntPtr jobHandle);

        /* IN-MEMORY BUFFERS */

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int qpdfjob_register_buffer_input(IntPtr jobHandle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8StringMarshaller))] string name, IntPtr data, nuint length);

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int qpdfjob_register_buffer_output(IntPtr jobHandle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8StringMarshaller))] string name, IntPtr callbackHandler, IntPtr udata);

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
