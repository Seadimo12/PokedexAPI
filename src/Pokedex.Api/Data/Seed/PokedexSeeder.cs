using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Pokedex.Api.Domain;
using Pokedex.Api.Dtos;
using Pokedex.Api.Mapping;

namespace Pokedex.Api.Data.Seed;

public static class PokedexSeeder
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task SeedAsync(PokedexDbContext db, IHostEnvironment env, CancellationToken cancellationToken = default)
    {
        if (await db.Pokemon.AnyAsync(cancellationToken))
        {
            return;
        }

        var seed = JsonSerializer.Deserialize<SeedFile>(await File.ReadAllTextAsync(FindSeedPath(env), cancellationToken), JsonOptions)
                   ?? throw new InvalidOperationException("pokemon.json is empty.");

        var types = seed.Pokemon
            .SelectMany(p => p.Types.Select(t => t.Name.ToLowerInvariant()))
            .Distinct()
            .Select(name => new PokeType { Name = name })
            .ToList();
        db.Types.AddRange(types);

        var abilities = seed.Pokemon
            .SelectMany(p => p.Abilities.Select(a => a.Name.ToLowerInvariant()))
            .Distinct()
            .Select(name => new Ability { Name = name })
            .ToList();
        db.Abilities.AddRange(abilities);
        await db.SaveChangesAsync(cancellationToken);

        var typeByName = types.ToDictionary(t => t.Name);
        var abilityByName = abilities.ToDictionary(a => a.Name);

        foreach (var row in seed.Pokemon)
        {
            var name = row.Name.ToLowerInvariant();
            var pokemon = new Pokemon
            {
                Id = row.NationalNumber,
                NationalNumber = row.NationalNumber,
                Name = name,
                Height = row.Height,
                Weight = row.Weight,
                BaseExperience = row.BaseExperience
            };

            foreach (var type in row.Types)
            {
                pokemon.Types.Add(new PokemonTypeSlot
                {
                    TypeId = typeByName[type.Name.ToLowerInvariant()].Id,
                    Slot = type.Slot
                });
            }

            foreach (var stat in new StatsDto(
                         row.Stats.Hp,
                         row.Stats.Attack,
                         row.Stats.Defense,
                         row.Stats.SpecialAttack,
                         row.Stats.SpecialDefense,
                         row.Stats.Speed).ToEntities())
            {
                pokemon.Stats.Add(stat);
            }

            foreach (var ability in row.Abilities)
            {
                pokemon.Abilities.Add(new PokemonAbility
                {
                    AbilityId = abilityByName[ability.Name.ToLowerInvariant()].Id,
                    IsHidden = ability.IsHidden
                });
            }

            db.Pokemon.Add(pokemon);
        }

        await db.SaveChangesAsync(cancellationToken);

        var byNationalNumber = await db.Pokemon.ToDictionaryAsync(p => p.NationalNumber, cancellationToken);
        foreach (var link in seed.Evolutions)
        {
            if (!byNationalNumber.TryGetValue(link.From, out var from) ||
                !byNationalNumber.TryGetValue(link.To, out var to))
            {
                continue;
            }

            db.Evolutions.Add(new Evolution
            {
                FromPokemonId = from.Id,
                ToPokemonId = to.Id,
                Trigger = link.Trigger,
                MinLevel = link.MinLevel
            });
        }

        await db.SaveChangesAsync(cancellationToken);
        await db.Database.ExecuteSqlRawAsync(
            """SELECT setval(pg_get_serial_sequence('pokemon', 'Id'), (SELECT COALESCE(MAX("Id"), 1) FROM pokemon))""",
            cancellationToken);
    }

    public static string FindSeedPath(IHostEnvironment env)
    {
        string[] candidates =
        [
            Path.Combine(AppContext.BaseDirectory, "data", "pokemon.json"),
            Path.Combine(env.ContentRootPath, "data", "pokemon.json"),
            Path.GetFullPath(Path.Combine(env.ContentRootPath, "..", "..", "data", "pokemon.json"))
        ];

        return candidates.FirstOrDefault(File.Exists)
               ?? throw new FileNotFoundException("Could not find data/pokemon.json. Run tools/fetch-gen1.py or copy the seed file.");
    }
}
