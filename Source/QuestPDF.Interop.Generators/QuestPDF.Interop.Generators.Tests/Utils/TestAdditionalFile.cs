using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace QuestPDF.Interop.Generators.Tests.Utils;

public class TestAdditionalFile : AdditionalText
{
    private readonly SourceText _text;

    public TestAdditionalFile(string path, string text)
    {
        Path = path;
        _text = SourceText.From(text);
    }

    public override SourceText GetText(CancellationToken cancellationToken = new()) => _text;

    public override string Path { get; }
}