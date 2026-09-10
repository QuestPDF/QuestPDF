using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace QuestPDF.Helpers;

internal enum ResourceArchiveCompression
{
    Gzip,
    Brotli
}

/// <summary>
/// Compressed container for resource files shipped with the library, e.g. the default fonts.
/// Written by the explicit <c>ResourceArchiveTests.RegenerateDefaultFontArchives</c> test, read at runtime by the code that owns the resource.
/// </summary>
/// <remarks>
/// Layout: magic bytes, format version, entry count, then for each entry its name, length and content.
/// Entries are read sequentially, straight into per-entry buffers, so the whole archive is never held in memory.
/// Brotli on net5.0 and newer, gzip on netstandard2.0.
/// </remarks>
internal static class ResourceArchive
{
    private static readonly byte[] Magic = "QUESTPDF-ARCHIVE"u8.ToArray();
    private const byte FormatVersion = 1;
    
#if NET5_0_OR_GREATER
    internal const ResourceArchiveCompression RuntimeCompression = ResourceArchiveCompression.Brotli;
#else
    internal const ResourceArchiveCompression RuntimeCompression = ResourceArchiveCompression.Gzip;
#endif
    
    /// <summary>
    /// File extension of the archive variant that the current target framework reads.
    /// </summary>
    internal static string RuntimeFileExtension => GetFileExtension(RuntimeCompression);
    
    internal static string GetFileExtension(ResourceArchiveCompression compression)
    {
        return compression switch
        {
            ResourceArchiveCompression.Gzip => ".gz",
            ResourceArchiveCompression.Brotli => ".br",
            _ => throw new ArgumentOutOfRangeException(nameof(compression))
        };
    }
    
#if NET6_0_OR_GREATER
    internal static void Write(Stream output, IReadOnlyCollection<(string Name, byte[] Content)> entries, ResourceArchiveCompression compression)
    {
        using var compressedStream = CreateCompressionStream(output, compression);
        using var writer = new BinaryWriter(compressedStream, Encoding.UTF8, leaveOpen: true);
        
        writer.Write(Magic);
        writer.Write(FormatVersion);
        writer.Write(entries.Count);
        
        foreach (var (name, content) in entries)
        {
            writer.Write(name);
            writer.Write(content.Length);
            writer.Write(content);
        }
    }
    
    private static Stream CreateCompressionStream(Stream output, ResourceArchiveCompression compression)
    {
        const CompressionLevel level = CompressionLevel.SmallestSize;
        
        return compression switch
        {
            ResourceArchiveCompression.Gzip => new GZipStream(output, level, leaveOpen: true),
            ResourceArchiveCompression.Brotli => new BrotliStream(output, level, leaveOpen: true),
            _ => throw new ArgumentOutOfRangeException(nameof(compression))
        };
    }
#endif
    
    /// <summary>
    /// Reads the archive variant matching the current target framework from a file.
    /// </summary>
    internal static IEnumerable<(string Name, byte[] Content)> Read(string path)
    {
        using var fileStream = File.OpenRead(path);
        
        foreach (var entry in Read(fileStream))
            yield return entry;
    }
    
    internal static IEnumerable<(string Name, byte[] Content)> Read(Stream input, ResourceArchiveCompression compression = RuntimeCompression)
    {
        using var decompressedStream = CreateDecompressionStream(input, compression);
        using var reader = new BinaryReader(decompressedStream, Encoding.UTF8, leaveOpen: true);
        
        var magic = reader.ReadBytes(Magic.Length);
        
        if (!Magic.AsSpan().SequenceEqual(magic))
            throw new InvalidDataException("The stream is not a QuestPDF resource archive.");
        
        var version = reader.ReadByte();
        
        if (version != FormatVersion)
            throw new InvalidDataException($"Unsupported QuestPDF resource archive version: {version}, expected: {FormatVersion}.");
        
        var count = reader.ReadInt32();
        
        for (var i = 0; i < count; i++)
        {
            var name = reader.ReadString();
            var length = reader.ReadInt32();
            var content = reader.ReadBytes(length);
            
            if (content.Length != length)
                throw new EndOfStreamException($"The QuestPDF resource archive is truncated: entry '{name}' is incomplete.");
            
            yield return (name, content);
        }
    }

    private static Stream CreateDecompressionStream(Stream input, ResourceArchiveCompression compression)
    {
        return compression switch
        {
            ResourceArchiveCompression.Gzip => new GZipStream(input, CompressionMode.Decompress, leaveOpen: true),
#if NET5_0_OR_GREATER
            ResourceArchiveCompression.Brotli => new BrotliStream(input, CompressionMode.Decompress, leaveOpen: true),
#endif
            _ => throw new NotSupportedException($"Compression {compression} is not supported on this target framework.")
        };
    }
}
