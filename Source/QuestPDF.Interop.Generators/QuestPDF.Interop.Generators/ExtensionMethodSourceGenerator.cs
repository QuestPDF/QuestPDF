using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators;

internal class ExtensionMethodSourceGenerator(Type targetType) : ObjectSourceGeneratorBase(targetType)
{
    protected override IEnumerable<IMethodSymbol> GetTargetMethods(Compilation compilation)
    {
        return compilation.GetAllQuestPdfExtensionMethods()
            .Where(x => !ExcludeMembers.Any(x.ToDisplayString().Contains))
            .FilterSupportedMethods()
            .Where(x => x.Parameters.First().Type.Name.Contains(TargetClrType.Name));
    }
}
