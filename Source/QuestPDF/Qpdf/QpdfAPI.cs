using System;
using System.Runtime.InteropServices;
using System.Text;
using QuestPDF.Skia;

namespace QuestPDF.Qpdf;

static class QpdfAPI
{
    public static string? GetQpdfVersion()
    {
        var ptr = API.qpdf_get_qpdf_version();
        return Marshal.PtrToStringAnsi(ptr);
    }
    
    public static void ExecuteJob(string jobJson)
    {
        QpdfNativeDependencyCompatibilityChecker.Test();
        
        // create StringBuilder that will store the error message
        var error = new StringBuilder();
        var errorHandle = GCHandle.Alloc(error);
        var errorPtr = GCHandle.ToIntPtr(errorHandle);
        
        // create logger
        var logger = API.qpdflogger_create();
        API.qpdflogger_set_error(logger, 4, LoggingCallbackPointer, errorPtr); // 4 = custom logger
        
        // perform the job
        var jobHandle = API.qpdfjob_init();
        API.qpdfjob_set_logger(jobHandle, logger);
        API.qpdfjob_initialize_from_json(jobHandle, jobJson);
        var jobResultId = API.qpdfjob_run(jobHandle);
        API.qpdfjob_cleanup(jobHandle);
        
        // logger cleanup
        API.qpdflogger_cleanup(logger);
        
        // check errors
        var isError = jobResultId is 2; // 0 = success, 1 = undefined, 2 = error, 3 = warning
        
        var errorMessage = error.ToString();
        errorHandle.Free();
        
        if (isError)
            throw new Exception($"QuestPDF could not perform document operation:\n\n{errorMessage}");
    }
    
    #region Logging
    
    private static int LoggingCallback(IntPtr data, int length, IntPtr udata)
    {
        var bytes = new byte[length];
        Marshal.Copy(data, bytes, 0, length);

        var handle = GCHandle.FromIntPtr(udata);
        var stringBuilder = (StringBuilder)handle.Target;
        stringBuilder?.Append(Encoding.ASCII.GetString(bytes));

        return 0;
    }
    
    private delegate int CallbackDelegate(IntPtr data, int length, IntPtr udata);
    private static readonly CallbackDelegate LoggingCallbackDelegate = LoggingCallback;
    private static readonly IntPtr LoggingCallbackPointer = Marshal.GetFunctionPointerForDelegate(LoggingCallbackDelegate);
    
    #endregion
    
    private static class API
    {
        const string LibraryName = "qpdf";
        
        /* GENERAL */
        
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr qpdf_get_qpdf_version();
    
        /* JOBS */
        
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr qpdfjob_init();
    
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void qpdfjob_cleanup(IntPtr jobHandle);
    
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int qpdfjob_initialize_from_json(IntPtr jobHandle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8StringMarshaller))] string json);

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int qpdfjob_run(IntPtr jobHandle);
        
        /* LOGGING */
        
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = nameof(qpdflogger_create))]
        public static extern IntPtr qpdflogger_create();

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = nameof(qpdflogger_cleanup))]
        public static extern void qpdflogger_cleanup(IntPtr loggerHandle);
        
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = nameof(qpdflogger_set_error))]
        public static extern void qpdflogger_set_error(IntPtr loggerHandle, int destination, IntPtr callBackHandler, IntPtr udata);
        
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = nameof(qpdfjob_set_logger))]
        public static extern void qpdfjob_set_logger(IntPtr jobHandle, IntPtr loggerHandle);
    }
}