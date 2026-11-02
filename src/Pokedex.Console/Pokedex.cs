namespace Pokedex.Console;

public sealed class Pokedex(IReadOnlyList<Pokemon> pokemon)
{
    public Pokemon? Find(string name) =>
        pokemon.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    public IReadOnlyList<Pokemon> OfType(PokemonType type) =>
        pokemon.Where(p => p.Types.Contains(type)).OrderBy(p => p.NationalNumber).ToList();

    public IReadOnlyList<Pokemon> Top(StatKind stat, int count) =>
        pokemon.OrderByDescending(p => p.Stats[stat]).ThenBy(p => p.NationalNumber).Take(count).ToList();
}
