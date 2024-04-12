using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Skia;

namespace QuestPDF.Helpers
{
    internal static class NativeDependencyCompatibilityChecker
    {
        private static bool IsCompatibilityChecked = false;
        
        public static void Test()
        {
            if (IsCompatibilityChecked)
                return;
            
            IsCompatibilityChecked = true;
            SkNativeDependencyCompatibilityChecker.Test();
        }
    }
}
