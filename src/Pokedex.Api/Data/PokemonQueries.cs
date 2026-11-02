using Microsoft.EntityFrameworkCore;
using Pokedex.Api.Domain;

namespace Pokedex.Api.Data;

public static class PokemonQueries
{
    public static IQueryable<Pokemon> WithDetails(this IQueryable<Pokemon> query) =>
        query
            .AsSplitQuery()
            .Include(p => p.Types).ThenInclude(t => t.Type)
            .Include(p => p.Stats)
            .Include(p => p.Abilities).ThenInclude(a => a.Ability);
}
