namespace Pokedex.Api.Domain;

public sealed class PokeType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<PokemonTypeSlot> Pokemon { get; set; } = new List<PokemonTypeSlot>();
}
