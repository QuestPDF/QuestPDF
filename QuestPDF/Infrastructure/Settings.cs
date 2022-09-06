namespace QuestPDF.Infrastructure
{
    public static class Settings
    {
        public static int DocumentLayoutExceptionThreshold { get; set; } = 250;
        
        public static bool EnableCaching { get; set; } = !System.Diagnostics.Debugger.IsAttached;
        public static bool EnableDebugging { get; set; } = System.Diagnostics.Debugger.IsAttached;
    }
}