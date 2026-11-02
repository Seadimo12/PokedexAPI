using Microsoft.EntityFrameworkCore;
using Pokedex.Api.Domain;

namespace Pokedex.Api.Data;

public sealed class PokedexDbContext(DbContextOptions<PokedexDbContext> options) : DbContext(options)
{
    public DbSet<Pokemon> Pokemon => Set<Pokemon>();
    public DbSet<PokeType> Types => Set<PokeType>();
    public DbSet<PokemonTypeSlot> PokemonTypes => Set<PokemonTypeSlot>();
    public DbSet<PokemonStat> PokemonStats => Set<PokemonStat>();
    public DbSet<Ability> Abilities => Set<Ability>();
    public DbSet<PokemonAbility> PokemonAbilities => Set<PokemonAbility>();
    public DbSet<Evolution> Evolutions => Set<Evolution>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PokedexDbContext).Assembly);
    }
}
