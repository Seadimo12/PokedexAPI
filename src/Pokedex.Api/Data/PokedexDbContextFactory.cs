using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Pokedex.Api.Data;

public sealed class PokedexDbContextFactory : IDesignTimeDbContextFactory<PokedexDbContext>
{
    public PokedexDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<PokedexDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=pokedex;Username=pokedex;Password=pokedex")
            .Options;

        return new PokedexDbContext(options);
    }
}
