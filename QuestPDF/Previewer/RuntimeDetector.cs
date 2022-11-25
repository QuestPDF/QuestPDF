namespace QuestPDF.Previewer;

public static class RuntimeDetector
{
#if NET6_0_OR_GREATER
    public static bool IsNet6OrGreater => true;
#else
    public static bool IsNet6OrGreater => false;
#endif
    
#if NETCOREAPP3_0_OR_GREATER
    public static bool IsNet3OrGreater => true;
#else
    public static bool IsNet6OrGreater => false;
#endif
}