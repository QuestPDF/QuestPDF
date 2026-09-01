using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace QuestPDF.Helpers
{
    internal static class DebugAssert
    {
        [Conditional("DEBUG")]
        public static void Finite(float value, [CallerArgumentExpression("value")] string? valueName = null)
        {
            if (float.IsNaN(value) || float.IsInfinity(value))
                Debug.Fail($"The {valueName} argument is {value} but must be a finite number.");
        }
    }
}
