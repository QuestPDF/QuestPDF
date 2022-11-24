using System;
using System.Linq;

namespace QuestPDF.Previewer;

internal static class UnitTestDetector
{
    public static bool RunningInUnitTest { get; }   

    private static string[] UnitTestAssemblies = { "xunit", "nunit", "mstest" };
    
    static UnitTestDetector()
    {
        RunningInUnitTest = AppDomain
            .CurrentDomain
            .GetAssemblies()
            .Select(x => x.FullName.ToLowerInvariant())
            .Any(x => UnitTestAssemblies.Any(x.Contains));
    }
}