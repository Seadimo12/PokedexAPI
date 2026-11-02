using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pokedex.Api.Domain;

namespace Pokedex.Api.Data.Configurations;

public sealed class PokemonConfiguration : IEntityTypeConfiguration<Pokemon>
{
    public void Configure(EntityTypeBuilder<Pokemon> builder)
    {
        builder.ToTable("pokemon");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).HasMaxLength(50).IsRequired();
        builder.HasIndex(p => p.NationalNumber).IsUnique();
        builder.HasIndex(p => p.Name).IsUnique();
    }
}

public sealed class PokeTypeConfiguration : IEntityTypeConfiguration<PokeType>
{
    public void Configure(EntityTypeBuilder<PokeType> builder)
    {
        builder.ToTable("types");
        builder.Property(t => t.Name).HasMaxLength(30).IsRequired();
        builder.HasIndex(t => t.Name).IsUnique();
    }
}

public sealed class PokemonTypeSlotConfiguration : IEntityTypeConfiguration<PokemonTypeSlot>
{
    public void Configure(EntityTypeBuilder<PokemonTypeSlot> builder)
    {
        builder.ToTable("pokemon_types");
        builder.HasKey(x => new { x.PokemonId, x.TypeId });
        builder.HasIndex(x => new { x.PokemonId, x.Slot }).IsUnique();

        builder.HasOne(x => x.Pokemon)
            .WithMany(p => p.Types)
            .HasForeignKey(x => x.PokemonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Type)
            .WithMany(t => t.Pokemon)
            .HasForeignKey(x => x.TypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class PokemonStatConfiguration : IEntityTypeConfiguration<PokemonStat>
{
    public void Configure(EntityTypeBuilder<PokemonStat> builder)
    {
        builder.ToTable("pokemon_stats");
        builder.HasKey(s => new { s.PokemonId, s.Kind });
        builder.Property(s => s.Kind).HasConversion<string>().HasMaxLength(20);

        builder.HasOne(s => s.Pokemon)
            .WithMany(p => p.Stats)
            .HasForeignKey(s => s.PokemonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class AbilityConfiguration : IEntityTypeConfiguration<Ability>
{
    public void Configure(EntityTypeBuilder<Ability> builder)
    {
        builder.ToTable("abilities");
        builder.Property(a => a.Name).HasMaxLength(50).IsRequired();
        builder.HasIndex(a => a.Name).IsUnique();
    }
}

public sealed class PokemonAbilityConfiguration : IEntityTypeConfiguration<PokemonAbility>
{
    public void Configure(EntityTypeBuilder<PokemonAbility> builder)
    {
        builder.ToTable("pokemon_abilities");
        builder.HasKey(x => new { x.PokemonId, x.AbilityId });

        builder.HasOne(x => x.Pokemon)
            .WithMany(p => p.Abilities)
            .HasForeignKey(x => x.PokemonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Ability)
            .WithMany(a => a.Pokemon)
            .HasForeignKey(x => x.AbilityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class EvolutionConfiguration : IEntityTypeConfiguration<Evolution>
{
    public void Configure(EntityTypeBuilder<Evolution> builder)
    {
        builder.ToTable("evolutions");
        builder.Property(e => e.Trigger).HasMaxLength(40).IsRequired();
        builder.HasIndex(e => new { e.FromPokemonId, e.ToPokemonId }).IsUnique();

        builder.HasOne(e => e.FromPokemon)
            .WithMany(p => p.EvolvesTo)
            .HasForeignKey(e => e.FromPokemonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.ToPokemon)
            .WithMany(p => p.EvolvesFrom)
            .HasForeignKey(e => e.ToPokemonId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
