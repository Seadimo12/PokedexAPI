using Pokedex.Api.Domain;
using Pokedex.Api.Dtos;

namespace Pokedex.Api.Mapping;

public static class PokemonMapper
{
    public static PokemonResponse ToResponse(this Pokemon pokemon) =>
        new(
            pokemon.Id,
            pokemon.NationalNumber,
            pokemon.Name,
            pokemon.Height,
            pokemon.Weight,
            pokemon.BaseExperience,
            pokemon.Types.OrderBy(t => t.Slot).Select(t => new TypeSlotDto(t.Type.Name, t.Slot)).ToList(),
            pokemon.Stats.ToDto(),
            pokemon.Abilities.Select(a => new AbilityDto(a.Ability.Name, a.IsHidden)).ToList());

    public static StatsDto ToDto(this IEnumerable<PokemonStat> stats)
    {
        var map = stats.ToDictionary(s => s.Kind, s => s.Value);
        return new StatsDto(
            map.GetValueOrDefault(StatKind.Hp),
            map.GetValueOrDefault(StatKind.Attack),
            map.GetValueOrDefault(StatKind.Defense),
            map.GetValueOrDefault(StatKind.SpecialAttack),
            map.GetValueOrDefault(StatKind.SpecialDefense),
            map.GetValueOrDefault(StatKind.Speed));
    }

    public static IEnumerable<PokemonStat> ToEntities(this StatsDto stats) =>
    [
        new() { Kind = StatKind.Hp, Value = stats.Hp },
        new() { Kind = StatKind.Attack, Value = stats.Attack },
        new() { Kind = StatKind.Defense, Value = stats.Defense },
        new() { Kind = StatKind.SpecialAttack, Value = stats.SpecialAttack },
        new() { Kind = StatKind.SpecialDefense, Value = stats.SpecialDefense },
        new() { Kind = StatKind.Speed, Value = stats.Speed }
    ];
}
