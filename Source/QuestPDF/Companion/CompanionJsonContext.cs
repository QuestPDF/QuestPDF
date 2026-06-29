#if NET8_0_OR_GREATER

using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Companion;

[JsonSourceGenerationOptions(MaxDepth = 256, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, Converters = new[] { typeof(CompanionLicenseTypeJsonConverter), typeof(CompanionSpacePlanTypeJsonConverter) })]
[JsonSerializable(typeof(CompanionCommands.Notify), TypeInfoPropertyName = "Notify")]
[JsonSerializable(typeof(CompanionCommands.GetVersionCommandResponse), TypeInfoPropertyName = "GetVersionCommandResponse")]
[JsonSerializable(typeof(ICollection<PageSnapshotIndex>), TypeInfoPropertyName = "PageSnapshotIndexCollection")]
[JsonSerializable(typeof(CompanionCommands.UpdateDocumentStructure), TypeInfoPropertyName = "UpdateDocumentStructure")]
[JsonSerializable(typeof(CompanionCommands.ProvideRenderedDocumentPage), TypeInfoPropertyName = "ProvideRenderedDocumentPage")]
[JsonSerializable(typeof(CompanionCommands.ShowGenericException), TypeInfoPropertyName = "ShowGenericException")]
internal sealed partial class CompanionJsonContext : JsonSerializerContext;

internal sealed class CompanionLicenseTypeJsonConverter() : JsonStringEnumConverter<LicenseType>(JsonNamingPolicy.CamelCase);

internal sealed class CompanionSpacePlanTypeJsonConverter() : JsonStringEnumConverter<SpacePlanType>(JsonNamingPolicy.CamelCase);

#endif
