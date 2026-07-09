#if NET5_0_OR_GREATER
using System.Text.Json;
using System.Text.Json.Serialization;
#endif

namespace QuestPDF.Qpdf;

static class QpdfJobSerializer
{
    public static string Serialize(JobConfiguration configuration)
    {
#if NET6_0_OR_GREATER
        return JsonSerializer.Serialize(configuration, QpdfJsonContext.Default.JobConfiguration);
#elif NET5_0_OR_GREATER
        return JsonSerializer.Serialize(configuration, JsonSerializerOptions);
#else
        return SimpleJsonSerializer.Serialize(configuration);
#endif
    }
    
#if NET5_0
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
    };
#endif
}
