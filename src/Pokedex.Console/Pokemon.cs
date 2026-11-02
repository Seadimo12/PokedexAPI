namespace Pokedex.Console;

public enum PokemonType
{
    Normal,
    Fire,
    Water,
    Electric,
    Grass,
    Ice,
    Fighting,
    Poison,
    Ground,
    Flying,
    Psychic,
    Bug,
    Rock,
    Ghost,
    Dragon,
    Dark,
    Steel,
    Fairy
}

public enum StatKind
{
    Hp,
    Attack,
    Defense,
    SpecialAttack,
    SpecialDefense,
    Speed
}

public sealed record Stats(int Hp, int Attack, int Defense, int SpecialAttack, int SpecialDefense, int Speed)
{
    public int this[StatKind kind] => kind switch
    {
        StatKind.Hp => Hp,
        StatKind.Attack => Attack,
        StatKind.Defense => Defense,
        StatKind.SpecialAttack => SpecialAttack,
        StatKind.SpecialDefense => SpecialDefense,
        StatKind.Speed => Speed,
        _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown stat.")
    };
}

public sealed record Pokemon(
    int NationalNumber,
    string Name,
    IReadOnlyList<PokemonType> Types,
    Stats Stats);
