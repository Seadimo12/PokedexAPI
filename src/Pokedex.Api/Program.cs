using Microsoft.EntityFrameworkCore;
using Pokedex.Api.Data;
using Pokedex.Api.Data.Seed;
using Pokedex.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PokedexDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Pokedex"));
    if (builder.Environment.IsDevelopment())
    {
        options.LogTo(Console.WriteLine, LogLevel.Information);
        options.EnableSensitiveDataLogging();
    }
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PokedexDbContext>();
    await db.Database.MigrateAsync();
    await PokedexSeeder.SeedAsync(db, app.Environment);
}

app.MapGet("/", () => Results.Ok(new
{
    name = "Pokédex API",
    chapter = 2,
    docs = new[] { "GET /pokemon", "GET /pokemon/{id}", "GET /pokemon/{id}/evolutions" }
}));

app.MapPokemonEndpoints();
app.Run();
