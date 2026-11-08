using System.Text.Json.Serialization;

namespace SkEditorAssetGenerator;

[JsonSerializable(typeof(string[]))]
[JsonSerializable(typeof(Dictionary<string, string>))]
internal partial class ResourceJsonContext : JsonSerializerContext;