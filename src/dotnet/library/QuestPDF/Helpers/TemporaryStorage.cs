using System;
using System.IO;
using System.Linq;

namespace QuestPDF.Helpers;

internal static class TemporaryStorage
{
    private static string? TemporaryStoragePath { get; set; }

    internal static string GetPath()
    {
        var path = TryGetPath();

        if (path == null)
        {
            throw new InvalidOperationException(
                "Unable to find a suitable temporary storage location. " +
                "Please specify it using the Settings.TemporaryStoragePath setting and ensure that the application has permissions to read and write to that location.");
        }
        
        return path;
    }
    
    internal static string? TryGetPath()
    {
        if (TemporaryStoragePath != null)
            return TemporaryStoragePath;
            
        var candidates = new[]
        {
            Settings.TemporaryStoragePath,
            Path.Combine(Path.GetTempPath(), "QuestPDF", "temp"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "QuestPDF", "temp"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "QuestPDF", "temp")
        };

        TemporaryStoragePath = candidates
            .Where(x => x != null)
            .FirstOrDefault(HasPermissionsToAlterPath);
        
        return TemporaryStoragePath;
    }
        
    private static bool HasPermissionsToAlterPath(string path)
    {
        try
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
                
            var testFile = Path.Combine(path, Path.GetRandomFileName());
                
            using (var fileStream = File.Create(testFile))
            {
                fileStream.WriteByte(123); // write anything
            }
                
            File.Delete(testFile);
                
            return true;
        }
        catch
        {
            return false;
        }
    }
}