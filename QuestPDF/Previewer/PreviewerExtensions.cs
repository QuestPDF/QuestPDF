#if NET6_0_OR_GREATER

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Previewer
{
    public static class Extensions
    {
        public static async Task ShowInPreviewer(this IDocument document, int port = 12500)
        {
            var previewerService = new PreviewerService(port);
            await previewerService.Connect();
            await RefreshPreview();
            
            HotReloadManager.UpdateApplicationRequested += (_, _) => RefreshPreview();

            while (true)
                await Task.Delay(TimeSpan.FromSeconds(1));

            Task RefreshPreview()
            {
                var pictures = GetPictures();
                return previewerService.RefreshPreview(pictures);
                
                ICollection<PreviewerPicture> GetPictures()
                {
                    try
                    {
                        return DocumentGenerator.GeneratePreviewerPictures(document);
                    }
                    catch (Exception exception)
                    {
                        var exceptionDocument = new ExceptionDocument(exception);
                        return DocumentGenerator.GeneratePreviewerPictures(exceptionDocument);
                    }
                }
            }
        }
    }
}

#endif
