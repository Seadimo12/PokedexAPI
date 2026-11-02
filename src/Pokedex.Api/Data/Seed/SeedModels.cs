using System.Text.Json.Serialization;

namespace Pokedex.Api.Data.Seed;

public sealed class SeedFile
{
    public List<SeedPokemon> Pokemon { get; set; } = [];
    public List<SeedEvolution> Evolutions { get; set; } = [];
}

public sealed class SeedPokemon
{
    public int NationalNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Height { get; set; }
    public int Weight { get; set; }
    public int BaseExperience { get; set; }
    public List<SeedTypeSlot> Types { get; set; } = [];
    public SeedStats Stats { get; set; } = new();
    public List<SeedAbility> Abilities { get; set; } = [];
}

public sealed class SeedTypeSlot
{
    public string Name { get; set; } = string.Empty;
    public int Slot { get; set; }
}

public sealed class SeedStats
{
    public int Hp { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }
    public int SpecialAttack { get; set; }
    public int SpecialDefense { get; set; }
    public int Speed { get; set; }
}

public sealed class SeedAbility
{
    public string Name { get; set; } = string.Empty;
    public bool IsHidden { get; set; }
}

public sealed class SeedEvolution
{
    [JsonPropertyName("from")]
    public int From { get; set; }

    [JsonPropertyName("to")]
    public int To { get; set; }

    public string Trigger { get; set; } = string.Empty;
    public int? MinLevel { get; set; }
}
