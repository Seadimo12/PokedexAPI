using Microsoft.EntityFrameworkCore;
using Pokedex.Api.Domain;
using Pokedex.Api.Dtos;
using Pokedex.Api.Mapping;

namespace Pokedex.Api.Data;

public static class PokemonWriter
{
    public static async Task ApplyAsync(PokedexDbContext db, Pokemon pokemon, UpsertPokemonRequest request, CancellationToken cancellationToken)
    {
        pokemon.NationalNumber = request.NationalNumber;
        pokemon.Name = request.Name.Trim().ToLowerInvariant();
        pokemon.Height = request.Height;
        pokemon.Weight = request.Weight;
        pokemon.BaseExperience = request.BaseExperience;

        pokemon.Types.Clear();
        pokemon.Stats.Clear();
        pokemon.Abilities.Clear();

        foreach (var slot in request.Types)
        {
            pokemon.Types.Add(new PokemonTypeSlot
            {
                Type = await GetOrCreateTypeAsync(db, slot.Name, cancellationToken),
                Slot = slot.Slot
            });
        }

        foreach (var stat in request.Stats.ToEntities())
        {
            pokemon.Stats.Add(stat);
        }

        foreach (var ability in request.Abilities)
        {
            pokemon.Abilities.Add(new PokemonAbility
            {
                Ability = await GetOrCreateAbilityAsync(db, ability.Name, cancellationToken),
                IsHidden = ability.IsHidden
            });
        }
    }

    private static async Task<PokeType> GetOrCreateTypeAsync(PokedexDbContext db, string name, CancellationToken cancellationToken)
    {
        var key = name.Trim().ToLowerInvariant();
        return db.Types.Local.FirstOrDefault(t => t.Name == key)
               ?? await db.Types.FirstOrDefaultAsync(t => t.Name == key, cancellationToken)
               ?? db.Types.Add(new PokeType { Name = key }).Entity;
    }

    private static async Task<Ability> GetOrCreateAbilityAsync(PokedexDbContext db, string name, CancellationToken cancellationToken)
    {
        var key = name.Trim().ToLowerInvariant();
        return db.Abilities.Local.FirstOrDefault(a => a.Name == key)
               ?? await db.Abilities.FirstOrDefaultAsync(a => a.Name == key, cancellationToken)
               ?? db.Abilities.Add(new Ability { Name = key }).Entity;
    }
}
