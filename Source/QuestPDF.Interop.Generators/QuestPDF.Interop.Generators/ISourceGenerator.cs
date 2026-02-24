using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators;

public interface IInteropSourceGenerator
{
    string GenerateCode(Compilation compilation, string language);
}