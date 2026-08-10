using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using QuestPDF.Companion;

#if NET8_0_OR_GREATER
[assembly: System.Reflection.Metadata.MetadataUpdateHandler(typeof(QuestPDF.Infrastructure.SourceCodePath))]
#endif

namespace QuestPDF.Infrastructure;

internal readonly struct SourceCodePath(StackFrame frame)
{
    public readonly string FilePath = frame.GetFileName() ?? string.Empty;
    public readonly int LineNumber = frame.GetFileLineNumber();

    internal static SourceCodePath? CreateFromCurrentStackTrace()
    {
        #if NET8_0_OR_GREATER
        
        if (!CompanionService.IsCompanionAttached)
            return null;

        // after hot reload, the runtime does not update debug information of edited method bodies,
        // so captured stack traces may point to stale source code locations;
        // known runtime limitation without a scheduled fix: https://github.com/dotnet/runtime/issues/56335
        if (CompanionService.IsDocumentHotReloaded)
            return null;
        
        return CaptureUserCodeLocation();

        #else
        
        return null;
        
        #endif
    }

    #if NET8_0_OR_GREATER

    private static readonly Module QuestPdfModule = typeof(SourceCodePath).Module;

    /// <summary>
    /// Memoizes resolution results per call site, identified by the method and the IL offset of the call instruction within it.
    /// Null values are cached as well: call sites without debug symbols should not be resolved repeatedly.
    /// </summary>
    internal static readonly ConcurrentDictionary<(RuntimeMethodHandle MethodHandle, int IlOffset), SourceCodePath?> CallSiteCache = new();

    /// <summary>
    /// Invoked by the runtime after a hot-reload metadata update, method-name-based convention!
    /// Hot reload re-emits edited method bodies, so cached IL offsets could map to wrong source lines.
    /// </summary>
    public static void ClearCache(Type[]? updatedTypes)
    {
        CallSiteCache.Clear();
    }

    private readonly record struct UserCodeCallSite(int FrameIndex, RuntimeMethodHandle MethodHandle, int IlOffset);
    
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static SourceCodePath? CaptureUserCodeLocation()
    {
        var frames = new StackTrace(fNeedFileInfo: false).GetFrames();
        
        var callSite = FindUserCodeCallSite(frames);

        if (callSite == null)
            return null;

        var (frameIndex, methodHandle, ilOffset) = callSite.Value;
        var cacheKey = (methodHandle, ilOffset);
        
        if (CallSiteCache.TryGetValue(cacheKey, out var cachedPath))
            return cachedPath;

        var resolvedFrame = new StackFrame(skipFrames: frameIndex, needFileInfo: true);
        var path = CreateFromResolvedFrame(resolvedFrame);

        CallSiteCache.TryAdd(cacheKey, path);
        return path;
    }

    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "The companion preview is a development-time feature. When method metadata is trimmed, the frame is skipped and the element simply carries no source location.")]
    private static UserCodeCallSite? FindUserCodeCallSite(StackFrame[] frames)
    {
        for (var frameIndex = 0; frameIndex < frames.Length; frameIndex++)
        {
            var method = frames[frameIndex].GetMethod();

            if (method == null || method.Module == QuestPdfModule)
                continue;

            if (!TryGetMethodHandle(method, out var methodHandle))
                continue;

            return new UserCodeCallSite(frameIndex, methodHandle, frames[frameIndex].GetILOffset());
        }

        return null;
    }

    private static bool TryGetMethodHandle(MethodBase method, out RuntimeMethodHandle methodHandle)
    {
        try
        {
            methodHandle = method.MethodHandle;
            return true;
        }
        catch (Exception exception) when (exception is InvalidOperationException or NotSupportedException)
        {
            // dynamically emitted code (e.g. a compiled expression tree) does not expose a method handle
            // and never carries debug symbols, deeper frames may still contain a user call site
            methodHandle = default;
            return false;
        }
    }

    private static SourceCodePath? CreateFromResolvedFrame(StackFrame frame)
    {
        return frame.GetFileName() != null
            ? new SourceCodePath(frame)
            : null;
    }

    #endif
}
