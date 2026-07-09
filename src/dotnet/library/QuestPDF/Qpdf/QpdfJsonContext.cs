#if NET6_0_OR_GREATER

using System.Text.Json.Serialization;

namespace QuestPDF.Qpdf;

[JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault)]
[JsonSerializable(typeof(JobConfiguration))]
internal sealed partial class QpdfJsonContext : JsonSerializerContext;

#endif
