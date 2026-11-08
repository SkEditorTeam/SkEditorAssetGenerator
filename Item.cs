using MemoryPack;

namespace SkEditorAssetGenerator;

[MemoryPackable]
public readonly partial struct Item(string id, string displayName, byte[] texture)
{
    public string Id { get; } = id;
    public string DisplayName { get; } = displayName;
    public byte[] Texture { get; } = texture;
}