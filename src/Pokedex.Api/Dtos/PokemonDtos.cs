namespace Pokedex.Api.Dtos;

public sealed record TypeSlotDto(string Name, int Slot);
public sealed record AbilityDto(string Name, bool IsHidden);
public sealed record StatsDto(int Hp, int Attack, int Defense, int SpecialAttack, int SpecialDefense, int Speed);

public sealed record PokemonResponse(
    int Id,
    int NationalNumber,
    string Name,
    int Height,
    int Weight,
    int BaseExperience,
    IReadOnlyList<TypeSlotDto> Types,
    StatsDto Stats,
    IReadOnlyList<AbilityDto> Abilities);

public sealed record PokemonListItemDto(
    int Id,
    int NationalNumber,
    string Name,
    IReadOnlyList<string> Types);

public sealed record UpsertPokemonRequest(
    int NationalNumber,
    string Name,
    int Height,
    int Weight,
    int BaseExperience,
    IReadOnlyList<TypeSlotDto> Types,
    StatsDto Stats,
    IReadOnlyList<AbilityDto> Abilities);

public sealed record EvolutionNodeDto(
    int Id,
    string Name,
    string? Trigger,
    int? MinLevel,
    IReadOnlyList<EvolutionNodeDto> EvolvesTo);

public sealed record EvolutionChainResponse(int PokemonId, string Name, EvolutionNodeDto Chain);
