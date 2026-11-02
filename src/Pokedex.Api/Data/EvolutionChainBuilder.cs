using Microsoft.EntityFrameworkCore;
using Pokedex.Api.Domain;
using Pokedex.Api.Dtos;

namespace Pokedex.Api.Data;

public static class EvolutionChainBuilder
{
    public static async Task<EvolutionChainResponse?> BuildAsync(PokedexDbContext db, int pokemonId, CancellationToken cancellationToken)
    {
        var species = await db.Pokemon.AsNoTracking().FirstOrDefaultAsync(p => p.Id == pokemonId, cancellationToken);
        if (species is null)
        {
            return null;
        }

        var links = await db.Evolutions.AsNoTracking().ToListAsync(cancellationToken);
        var names = await db.Pokemon.AsNoTracking()
            .Select(p => new { p.Id, p.Name })
            .ToDictionaryAsync(p => p.Id, p => p.Name, cancellationToken);

        var rootId = WalkToRoot(species.Id, links);
        var node = BuildNode(rootId, names, links, trigger: null, minLevel: null);
        return new EvolutionChainResponse(species.Id, species.Name, node);
    }

    private static int WalkToRoot(int id, List<Evolution> links)
    {
        var current = id;
        var seen = new HashSet<int> { current };
        while (true)
        {
            var parent = links.Find(l => l.ToPokemonId == current);
            if (parent is null || !seen.Add(parent.FromPokemonId))
            {
                return current;
            }

            current = parent.FromPokemonId;
        }
    }

    private static EvolutionNodeDto BuildNode(
        int id,
        Dictionary<int, string> names,
        List<Evolution> links,
        string? trigger,
        int? minLevel)
    {
        var children = links
            .Where(l => l.FromPokemonId == id)
            .Select(l => BuildNode(l.ToPokemonId, names, links, l.Trigger, l.MinLevel))
            .ToList();

        return new EvolutionNodeDto(id, names.GetValueOrDefault(id, "?"), trigger, minLevel, children);
    }
}
