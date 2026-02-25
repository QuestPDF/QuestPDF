using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators;

public static partial class ObsoleteMethodHelper
{
    private static readonly Version ObsoleteVersionThreshold = new(2025, 1, 0);
    
    [GeneratedRegex(@"(?<year>20\d{2})\.(?<month>0?[1-9]|1[0-2])")]
    private static partial Regex ObsoleteVersionRegex();

    public static IEnumerable<IMethodSymbol> ExcludeOldObsoleteMethods(this IEnumerable<IMethodSymbol> methodSymbols)
    {
        return methodSymbols.Where(method => !IsOldObsolete(method));

        static bool IsOldObsolete(IMethodSymbol method)
        {
            var deprecationMessage = method.TryGetDeprecationMessage();

            if (deprecationMessage is null)
                return false;

            var match = ObsoleteVersionRegex().Match(deprecationMessage);

            if (!match.Success)
                return false;
            
            var obsoleteSinceVersion = new Version(int.Parse(match.Groups["year"].Value), int.Parse(match.Groups["month"].Value), 0);

            return obsoleteSinceVersion < ObsoleteVersionThreshold;
        }
    }
    
    public static string? TryGetDeprecationMessage(this ISymbol symbol)
    {
        return symbol
            .GetAttributes()
            .FirstOrDefault(x => x.AttributeClass?.Name == "ObsoleteAttribute")
            ?.ConstructorArguments
            .FirstOrDefault().Value as string;
    }
}