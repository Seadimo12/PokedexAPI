namespace Pokedex.Api.Domain;

public sealed class PokemonTypeSlot
{
    public int PokemonId { get; set; }
    public int TypeId { get; set; }
    public int Slot { get; set; }

    public Pokemon Pokemon { get; set; } = null!;
    public PokeType Type { get; set; } = null!;
}
