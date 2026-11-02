-- Full evolution family for a Pokémon id (:id).
-- Walks to the root, then collects every descendant.
-- Equivalent to EvolutionChainBuilder in the API.

WITH RECURSIVE
ancestors AS (
    SELECT "FromPokemonId" AS id
    FROM evolutions
    WHERE "ToPokemonId" = :id
    UNION
    SELECT e."FromPokemonId"
    FROM evolutions e
    JOIN ancestors a ON e."ToPokemonId" = a.id
),
root AS (
    SELECT COALESCE((SELECT id FROM ancestors
                     WHERE id NOT IN (SELECT "ToPokemonId" FROM evolutions)
                     LIMIT 1), :id) AS id
),
family AS (
    SELECT r.id
    FROM root r
    UNION
    SELECT e."ToPokemonId"
    FROM evolutions e
    JOIN family f ON e."FromPokemonId" = f.id
)
SELECT p."Id", p."Name", e."Trigger", e."MinLevel", e."FromPokemonId"
FROM pokemon p
JOIN family f ON f.id = p."Id"
LEFT JOIN evolutions e ON e."ToPokemonId" = p."Id"
ORDER BY p."NationalNumber";
