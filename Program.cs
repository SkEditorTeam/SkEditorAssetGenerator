using System.Text.Json;
using MemoryPack;

namespace SkEditorAssetGenerator;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        if (args.Length < 3)
        {
            Console.WriteLine(
                "Usage: SkEditorAssetGenerator <assetsFolder> <langFile> <outputFilePath>");
            return 1;
        }

        string assetsFolderPath = args[0];
        string langFilePath = args[1];
        string outputFilePath = args[2];

        if (!Directory.Exists(assetsFolderPath))
        {
            await Console.Error.WriteLineAsync($"Assets folder path '{assetsFolderPath}' does not exist.");
            return 1;
        }

        if (!File.Exists(langFilePath))
        {
            await Console.Error.WriteLineAsync($"Language file path '{langFilePath}' does not exist.");
            return 1;
        }

        Console.WriteLine("Loading language file...");
        await using FileStream langStream = File.OpenRead(langFilePath);
        var allTranslations = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(langStream,
            ResourceJsonContext.Default.DictionaryStringString);

        if (allTranslations is null)
        {
            await Console.Error.WriteLineAsync("Failed to deserialize language file.");
            return 1;
        }

        Console.WriteLine("Processing asset files...");
        string[] files = Directory.GetFiles(assetsFolderPath, "*.png");
        
        List<Item> items = new(files.Length);

        foreach (string file in files)
        {
            string id = Path.GetFileNameWithoutExtension(file).ToLowerInvariant();

            if (!allTranslations.TryGetValue($"item.minecraft.{id}", out string? translation) &&
                !allTranslations.TryGetValue($"block.minecraft.{id}", out translation))
            {
                continue;
            }

            byte[] iconBytes = await File.ReadAllBytesAsync(file);
            items.Add(new Item(id, translation, iconBytes));
        }

        Console.WriteLine($"Successfully mapped {items.Count} items.");

        string? outputDir = Path.GetDirectoryName(outputFilePath);
        if (!string.IsNullOrEmpty(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        await using FileStream outputStream = File.Create(outputFilePath);
        await MemoryPackSerializer.SerializeAsync(outputStream, new ItemCache(items.ToArray()));

        Console.WriteLine(
            $"Generated cache saved to '{outputFilePath}' ({new FileInfo(outputFilePath).Length / 1024} KB).");
        return 0;
    }
}