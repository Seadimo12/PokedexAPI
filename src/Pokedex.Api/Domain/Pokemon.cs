namespace Pokedex.Api.Domain;

public sealed class Pokemon
{
    public int Id { get; set; }
    public int NationalNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Height { get; set; }
    public int Weight { get; set; }
    public int BaseExperience { get; set; }

    public ICollection<PokemonTypeSlot> Types { get; set; } = new List<PokemonTypeSlot>();
    public ICollection<PokemonStat> Stats { get; set; } = new List<PokemonStat>();
    public ICollection<PokemonAbility> Abilities { get; set; } = new List<PokemonAbility>();
    public ICollection<Evolution> EvolvesTo { get; set; } = new List<Evolution>();
    public ICollection<Evolution> EvolvesFrom { get; set; } = new List<Evolution>();
}
