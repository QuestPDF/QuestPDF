using System.Diagnostics;
using Avalonia.Controls;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.Previewer
{
    internal static class PreviewerUtils
    {
        public static async Task<bool> SavePdfWithDialog(IDocument? document, Window dialogOwner)
        {
            if (document == null)
                return false;
            
            var dialog = new SaveFileDialog()
            {
                DefaultExtension = ".pdf",
                InitialFileName = document.GetMetadata().Title ?? "Document",
                Filters = new List<FileDialogFilter>()
                {
                    new FileDialogFilter()
                    {
                        Extensions = new List<string>() { "pdf" },
                    }
                }
            };

            var filePath = await dialog.ShowAsync(dialogOwner);
            if (string.IsNullOrWhiteSpace(filePath))
                return false;

            var dirPath = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(dirPath))
                return false;

            //TODO Catch layout exceptions.
            document.GeneratePdf(filePath);
            Process.Start("explorer.exe", filePath);
            return true;
        }
    }
}
