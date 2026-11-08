using MemoryPack;

namespace SkEditorAssetGenerator;

[MemoryPackable]
public partial record ItemCache(Item[] Items);