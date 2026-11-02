using System.Text.Json;

namespace Pokedex.Console;

public static class CatalogLoader
{
    public static IReadOnlyList<Pokemon> Load(string path)
    {
        using var stream = File.OpenRead(path);
        using var doc = JsonDocument.Parse(stream);
        return doc.RootElement.GetProperty("pokemon")
            .EnumerateArray()
            .Select(ReadPokemon)
            .ToList();
    }

    public static string FindSeedPath()
    {
        string[] candidates =
        [
            Path.Combine(AppContext.BaseDirectory, "data", "pokemon.json"),
            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "data", "pokemon.json"))
        ];

        return candidates.FirstOrDefault(File.Exists)
               ?? throw new FileNotFoundException("Could not find data/pokemon.json.");
    }

    private static Pokemon ReadPokemon(JsonElement element)
    {
        var stats = element.GetProperty("stats");
        var types = element.GetProperty("types")
            .EnumerateArray()
            .OrderBy(t => t.GetProperty("slot").GetInt32())
            .Select(t => Enum.Parse<PokemonType>(t.GetProperty("name").GetString()!, ignoreCase: true))
            .ToList();

        return new Pokemon(
            element.GetProperty("nationalNumber").GetInt32(),
            element.GetProperty("name").GetString()!,
            types,
            new Stats(
                stats.GetProperty("hp").GetInt32(),
                stats.GetProperty("attack").GetInt32(),
                stats.GetProperty("defense").GetInt32(),
                stats.GetProperty("specialAttack").GetInt32(),
                stats.GetProperty("specialDefense").GetInt32(),
                stats.GetProperty("speed").GetInt32()));
    }
}
