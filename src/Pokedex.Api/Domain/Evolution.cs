namespace Pokedex.Api.Domain;

public sealed class Evolution
{
    public int Id { get; set; }
    public int FromPokemonId { get; set; }
    public int ToPokemonId { get; set; }
    public string Trigger { get; set; } = string.Empty;
    public int? MinLevel { get; set; }

    public Pokemon FromPokemon { get; set; } = null!;
    public Pokemon ToPokemon { get; set; } = null!;
}
