using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using QuestPDF.Companion;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Previewer;

public static class PreviewerExtensions
{
    private const string ObsoleteMessage = "The Previewer application is no longer supprted. Please use a new QuestPDF Companion application by calling ShowInCompanion() or ShowInCompanionAsync() methods.";
    
    #if NET6_0_OR_GREATER
    
    [Obsolete(ObsoleteMessage)]
    [ExcludeFromCodeCoverage]
    public static void ShowInPreviewer(this IDocument document, int port = 12500)
    {
        throw new NotImplementedException(ObsoleteMessage);
    }
    
    [Obsolete(ObsoleteMessage)]
    [ExcludeFromCodeCoverage]
    public static Task ShowInPreviewerAsync(this IDocument document, int port = 12500, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException(ObsoleteMessage);
    }
    
    #else

    [Obsolete(ObsoleteMessage)]
    [ExcludeFromCodeCoverage]
    public static void ShowInPreviewer(this IDocument document, int port = 12500)
    {
        throw new NotSupportedException(ObsoleteMessage);
    }

    [Obsolete(ObsoleteMessage)]
    [ExcludeFromCodeCoverage]
    public static async Task ShowInPreviewerAsync(this IDocument document, int port = 12500, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException(ObsoleteMessage);
    }

    #endif
}