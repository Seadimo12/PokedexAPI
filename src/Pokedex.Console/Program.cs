using Pokedex.Console;

var pokedex = new Pokedex.Console.Pokedex(CatalogLoader.Load(CatalogLoader.FindSeedPath()));

if (args.Length > 0)
{
    Run(string.Join(' ', args), pokedex);
    return;
}

Console.WriteLine("Pokédex (Chapter 0). Commands: find, type, top, compare. Type quit to exit.");
while (true)
{
    Console.Write("> ");
    var line = Console.ReadLine();
    if (line is null || line.Equals("quit", StringComparison.OrdinalIgnoreCase) || line.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    Run(line, pokedex);
}

static void Run(string line, Pokedex.Console.Pokedex pokedex)
{
    try
    {
        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        switch (parts)
        {
            case ["find", var name]:
                PrintOne(pokedex.Find(name), name);
                break;
            case ["type", var typeName] when Enum.TryParse<PokemonType>(typeName, ignoreCase: true, out var type):
                foreach (var pokemon in pokedex.OfType(type))
                {
                    Console.WriteLine($"{pokemon.NationalNumber,3}  {pokemon.Name}  ({string.Join('/', pokemon.Types)})");
                }
                break;
            case ["type", _]:
                Console.WriteLine("Unknown type. Try: normal fire water electric grass ice fighting poison ground flying psychic bug rock ghost dragon dark steel fairy");
                break;
            case ["top", var statName, var countText] when TryParseStat(statName, out var stat) && int.TryParse(countText, out var count) && count > 0:
                foreach (var pokemon in pokedex.Top(stat, count))
                {
                    Console.WriteLine($"{pokemon.Stats[stat],3}  {pokemon.Name}");
                }
                break;
            case ["compare", var leftName, var rightName]:
                Compare(pokedex.Find(leftName), pokedex.Find(rightName), leftName, rightName);
                break;
            default:
                Console.WriteLine("Usage:");
                Console.WriteLine("  find pikachu");
                Console.WriteLine("  type fire");
                Console.WriteLine("  top attack 10");
                Console.WriteLine("  compare bulbasaur charmander");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Could not run that command: {ex.Message}");
    }
}

static bool TryParseStat(string value, out StatKind stat)
{
    var key = value.Replace("-", "", StringComparison.Ordinal).Replace("_", "", StringComparison.Ordinal);
    if (key.Equals("spatk", StringComparison.OrdinalIgnoreCase) || key.Equals("specialattack", StringComparison.OrdinalIgnoreCase))
    {
        stat = StatKind.SpecialAttack;
        return true;
    }

    if (key.Equals("spdef", StringComparison.OrdinalIgnoreCase) || key.Equals("specialdefense", StringComparison.OrdinalIgnoreCase))
    {
        stat = StatKind.SpecialDefense;
        return true;
    }

    return Enum.TryParse(key, ignoreCase: true, out stat);
}

static void PrintOne(Pokemon? pokemon, string requested)
{
    if (pokemon is null)
    {
        Console.WriteLine($"No Pokémon named '{requested}'.");
        return;
    }

    Console.WriteLine($"#{pokemon.NationalNumber:000} {pokemon.Name}");
    Console.WriteLine($"Types: {string.Join(", ", pokemon.Types)}");
    Console.WriteLine($"HP {pokemon.Stats.Hp}  Atk {pokemon.Stats.Attack}  Def {pokemon.Stats.Defense}  SpA {pokemon.Stats.SpecialAttack}  SpD {pokemon.Stats.SpecialDefense}  Spe {pokemon.Stats.Speed}");
}

static void Compare(Pokemon? left, Pokemon? right, string leftName, string rightName)
{
    if (left is null || right is null)
    {
        if (left is null) Console.WriteLine($"No Pokémon named '{leftName}'.");
        if (right is null) Console.WriteLine($"No Pokémon named '{rightName}'.");
        return;
    }

    Console.WriteLine($"{left.Name,-16} vs {right.Name}");
    WriteStat("HP", left.Stats.Hp, right.Stats.Hp);
    WriteStat("Attack", left.Stats.Attack, right.Stats.Attack);
    WriteStat("Defense", left.Stats.Defense, right.Stats.Defense);
    WriteStat("Sp. Atk", left.Stats.SpecialAttack, right.Stats.SpecialAttack);
    WriteStat("Sp. Def", left.Stats.SpecialDefense, right.Stats.SpecialDefense);
    WriteStat("Speed", left.Stats.Speed, right.Stats.Speed);
}

static void WriteStat(string label, int left, int right)
{
    var marker = left == right ? "=" : left > right ? "<" : ">";
    Console.WriteLine($"{left,4}  {label,-8} {marker} {right,4}");
}
