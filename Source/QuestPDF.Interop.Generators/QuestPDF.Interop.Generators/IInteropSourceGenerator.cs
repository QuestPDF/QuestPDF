using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators;

internal interface IInteropSourceGenerator
{
    string GenerateCode(Compilation compilation, string language);
}