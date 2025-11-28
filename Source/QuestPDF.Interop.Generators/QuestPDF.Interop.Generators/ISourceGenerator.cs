using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators;

public interface IInteropSourceGenerator
{
    string GenerateCSharpCode(Compilation compilation);
    string GeneratePythonCode(Compilation compilation);
    string GenerateJavaCode(Compilation compilation);
    string GenerateTypeScriptCode(Compilation compilation);
    string GenerateKotlinCode(Compilation compilation);
}