using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using QuestPDF.Helpers;

namespace QuestPDF.UnitTests
{
    public class ResourceArchiveTests
    {
        private static readonly string LibraryResourcesDirectory = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(GetSourcePath())!, "..", "QuestPDF", "Resources"));
        private static readonly string DefaultFontsDirectory = Path.Combine(LibraryResourcesDirectory, "LatoFont");
        private static readonly string DefaultFontsArchivePath = Path.Combine(LibraryResourcesDirectory, "QuestPDF.Fonts.Lato");
        
        private static (string Name, byte[] Content)[] ReadDefaultFontFiles()
        {
            return Directory
                .GetFiles(DefaultFontsDirectory)
                .OrderBy(Path.GetFileName, StringComparer.Ordinal)
                .Select(x => (Name: Path.GetFileName(x), Content: File.ReadAllBytes(x)))
                .ToArray();
        }
        
        [TestCase("gzip")]
        [TestCase("brotli")]
        public void RoundTrip(string compressionName)
        {
            var compression = ParseCompression(compressionName);
            
            var entries = new[]
            {
                (Name: "first.bin", Content: Enumerable.Range(0, 10_000).Select(x => (byte)x).ToArray()),
                (Name: "empty.bin", Content: Array.Empty<byte>()),
                (Name: "zażółć.txt", Content: "gęślą jaźń"u8.ToArray())
            };
            
            using var stream = new MemoryStream();
            ResourceArchive.Write(stream, entries, compression);
            stream.Position = 0;
            
            var result = ResourceArchive.Read(stream, compression).ToArray();
            
            Assert.That(result, Is.EqualTo(entries));
        }
        
        [Test]
        public void Read_ShouldThrowException_WhenStreamIsNotArchive()
        {
            using var stream = new MemoryStream();
            
            using (var gzip = new System.IO.Compression.GZipStream(stream, System.IO.Compression.CompressionMode.Compress, leaveOpen: true))
                gzip.Write("definitely not an archive"u8);
            
            stream.Position = 0;
            
            Assert.Throws<InvalidDataException>(() => ResourceArchive.Read(stream, ResourceArchiveCompression.Gzip).ToArray());
        }
        
        /// <summary>
        /// Guards against editing the font files without regenerating the shipped archives,
        /// and against the reader and writer drifting apart.
        /// </summary>
        [TestCase("gzip")]
        [TestCase("brotli")]
        public void DefaultFontArchives_MatchSourceFontFiles(string compressionName)
        {
            var compression = ParseCompression(compressionName);
            var expected = ReadDefaultFontFiles();
            var archivePath = DefaultFontsArchivePath + ResourceArchive.GetFileExtension(compression);
            
            using var stream = File.OpenRead(archivePath);
            var actual = ResourceArchive.Read(stream, compression).ToArray();
            
            Assert.That(actual, Is.EqualTo(expected));
        }
        
        /// <summary>
        /// Maintainer utility: regenerates QuestPDF/Resources/QuestPDF.Fonts.Lato.{br,gz} from QuestPDF/Resources/LatoFont
        /// Run it manually after changing the default font files, then commit the archives.
        /// </summary>
        [Test, Explicit]
        public void RegenerateDefaultFontArchives()
        {
            var entries = ReadDefaultFontFiles();
            Assert.That(entries, Is.Not.Empty);
            
            foreach (var compression in Enum.GetValues<ResourceArchiveCompression>())
            {
                var path = DefaultFontsArchivePath + ResourceArchive.GetFileExtension(compression);
                
                using (var output = File.Create(path))
                    ResourceArchive.Write(output, entries, compression);
                
                TestContext.Out.WriteLine($"{path}: {new FileInfo(path).Length / 1024} KB");
            }
        }
        
        private static ResourceArchiveCompression ParseCompression(string name) => Enum.Parse<ResourceArchiveCompression>(name, ignoreCase: true);
        
        private static string GetSourcePath([CallerFilePath] string path = "") => path;
    }
}
