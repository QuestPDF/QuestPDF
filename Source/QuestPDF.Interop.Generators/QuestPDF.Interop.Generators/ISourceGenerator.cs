using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators;

public interface IInteropSourceGenerator
{
    string GenerateCSharpCode(Compilation compilation);
    string GeneratePythonCode(Compilation compilation);
}