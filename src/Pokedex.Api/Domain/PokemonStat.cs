namespace Pokedex.Api.Domain;

public sealed class PokemonStat
{
    public int PokemonId { get; set; }
    public StatKind Kind { get; set; }
    public int Value { get; set; }

    public Pokemon Pokemon { get; set; } = null!;
}
