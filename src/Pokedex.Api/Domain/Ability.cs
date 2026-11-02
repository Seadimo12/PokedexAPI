namespace Pokedex.Api.Domain;

public sealed class Ability
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<PokemonAbility> Pokemon { get; set; } = new List<PokemonAbility>();
}
