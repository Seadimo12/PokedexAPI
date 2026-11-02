using Microsoft.EntityFrameworkCore;
using Pokedex.Api.Data;
using Pokedex.Api.Domain;
using Pokedex.Api.Dtos;
using Pokedex.Api.Mapping;

namespace Pokedex.Api.Endpoints;

public static class PokemonEndpoints
{
    public static void MapPokemonEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/pokemon", ListAsync);
        app.MapGet("/pokemon/{id:int}", GetByIdAsync);
        app.MapGet("/pokemon/by-name/{name}", GetByNameAsync);
        app.MapGet("/pokemon/{id:int}/evolutions", GetEvolutionsAsync);
        app.MapGet("/types/{type}/pokemon", ListByTypeAsync);
        app.MapPost("/pokemon", CreateAsync);
        app.MapPut("/pokemon/{id:int}", UpdateAsync);
        app.MapDelete("/pokemon/{id:int}", DeleteAsync);
    }

    private static async Task<IResult> ListAsync(PokedexDbContext db, CancellationToken cancellationToken)
    {
        var items = await db.Pokemon.AsNoTracking()
            .OrderBy(p => p.NationalNumber)
            .Select(p => new PokemonListItemDto(
                p.Id,
                p.NationalNumber,
                p.Name,
                p.Types.OrderBy(t => t.Slot).Select(t => t.Type.Name).ToList()))
            .ToListAsync(cancellationToken);

        return TypedResults.Ok(items);
    }

    private static async Task<IResult> GetByIdAsync(int id, PokedexDbContext db, CancellationToken cancellationToken)
    {
        var pokemon = await db.Pokemon.AsNoTracking().WithDetails()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        return pokemon is null ? TypedResults.NotFound() : TypedResults.Ok(pokemon.ToResponse());
    }

    private static async Task<IResult> GetByNameAsync(string name, PokedexDbContext db, CancellationToken cancellationToken)
    {
        var key = name.Trim().ToLowerInvariant();
        var pokemon = await db.Pokemon.AsNoTracking().WithDetails()
            .FirstOrDefaultAsync(p => p.Name == key, cancellationToken);

        return pokemon is null ? TypedResults.NotFound() : TypedResults.Ok(pokemon.ToResponse());
    }

    private static async Task<IResult> ListByTypeAsync(string type, PokedexDbContext db, CancellationToken cancellationToken)
    {
        var key = type.Trim().ToLowerInvariant();
        var items = await db.Pokemon.AsNoTracking()
            .Where(p => p.Types.Any(t => t.Type.Name == key))
            .OrderBy(p => p.NationalNumber)
            .Select(p => new PokemonListItemDto(
                p.Id,
                p.NationalNumber,
                p.Name,
                p.Types.OrderBy(t => t.Slot).Select(t => t.Type.Name).ToList()))
            .ToListAsync(cancellationToken);

        return TypedResults.Ok(items);
    }

    private static async Task<IResult> GetEvolutionsAsync(int id, PokedexDbContext db, CancellationToken cancellationToken)
    {
        var chain = await EvolutionChainBuilder.BuildAsync(db, id, cancellationToken);
        return chain is null ? TypedResults.NotFound() : TypedResults.Ok(chain);
    }

    private static async Task<IResult> CreateAsync(UpsertPokemonRequest request, PokedexDbContext db, CancellationToken cancellationToken)
    {
        if (Validate(request) is { } error)
        {
            return error;
        }

        var pokemon = new Pokemon();
        await PokemonWriter.ApplyAsync(db, pokemon, request, cancellationToken);
        db.Pokemon.Add(pokemon);

        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.IsUniqueViolation())
        {
            return Conflict();
        }

        var created = await db.Pokemon.AsNoTracking().WithDetails()
            .FirstAsync(p => p.Id == pokemon.Id, cancellationToken);

        return TypedResults.Created($"/pokemon/{created.Id}", created.ToResponse());
    }

    private static async Task<IResult> UpdateAsync(int id, UpsertPokemonRequest request, PokedexDbContext db, CancellationToken cancellationToken)
    {
        if (Validate(request) is { } error)
        {
            return error;
        }

        var pokemon = await db.Pokemon
            .Include(p => p.Types)
            .Include(p => p.Stats)
            .Include(p => p.Abilities)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (pokemon is null)
        {
            return TypedResults.NotFound();
        }

        await PokemonWriter.ApplyAsync(db, pokemon, request, cancellationToken);

        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.IsUniqueViolation())
        {
            return Conflict();
        }

        return TypedResults.NoContent();
    }

    private static async Task<IResult> DeleteAsync(int id, PokedexDbContext db, CancellationToken cancellationToken)
    {
        var pokemon = await db.Pokemon.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (pokemon is null)
        {
            return TypedResults.NotFound();
        }

        await db.Evolutions
            .Where(e => e.FromPokemonId == id || e.ToPokemonId == id)
            .ExecuteDeleteAsync(cancellationToken);

        db.Pokemon.Remove(pokemon);
        await db.SaveChangesAsync(cancellationToken);
        return TypedResults.NoContent();
    }

    private static IResult? Validate(UpsertPokemonRequest request)
    {
        if (request.NationalNumber <= 0 || string.IsNullOrWhiteSpace(request.Name))
        {
            return TypedResults.BadRequest("nationalNumber must be positive and name is required.");
        }

        if (request.Types is null || request.Types.Count == 0 || request.Stats is null || request.Abilities is null)
        {
            return TypedResults.BadRequest("types, stats and abilities are required.");
        }

        return null;
    }

    private static IResult Conflict() =>
        TypedResults.Conflict(new { error = "A Pokémon with that national number or name already exists." });
}
