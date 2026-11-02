"""Download Gen 1 species, stats, abilities and evolution links from PokéAPI."""
from __future__ import annotations

import json
import time
import urllib.request
from pathlib import Path

BASE = "https://pokeapi.co/api/v2"
OUT = Path(__file__).resolve().parents[1] / "data" / "pokemon.json"


def get(url: str) -> dict:
    req = urllib.request.Request(url, headers={"User-Agent": "PokedexAPI/0.1 (learning project)"})
    with urllib.request.urlopen(req, timeout=60) as resp:
        return json.loads(resp.read().decode("utf-8"))


def main() -> None:
    pokemon: list[dict] = []
    evolutions: list[dict] = []
    seen_evo: set[tuple[int, int]] = set()
    chain_cache: dict[str, dict] = {}

    for n in range(1, 152):
        p = get(f"{BASE}/pokemon/{n}")
        stats = {s["stat"]["name"].replace("-", "_"): s["base_stat"] for s in p["stats"]}
        pokemon.append(
            {
                "nationalNumber": p["id"],
                "name": p["name"],
                "height": p["height"],
                "weight": p["weight"],
                "baseExperience": p["base_experience"] or 0,
                "types": [{"name": t["type"]["name"], "slot": t["slot"]} for t in p["types"]],
                "stats": {
                    "hp": stats["hp"],
                    "attack": stats["attack"],
                    "defense": stats["defense"],
                    "specialAttack": stats["special_attack"],
                    "specialDefense": stats["special_defense"],
                    "speed": stats["speed"],
                },
                "abilities": [
                    {"name": a["ability"]["name"], "isHidden": a["is_hidden"]}
                    for a in p["abilities"]
                ],
            }
        )

        species = get(p["species"]["url"])
        chain_url = species["evolution_chain"]["url"]
        if chain_url not in chain_cache:
            chain_cache[chain_url] = get(chain_url)
            time.sleep(0.05)
        walk_chain(chain_cache[chain_url]["chain"], evolutions, seen_evo)
        print(f"fetched {n:3d} {p['name']}", flush=True)
        time.sleep(0.05)

    OUT.parent.mkdir(parents=True, exist_ok=True)
    OUT.write_text(
        json.dumps({"pokemon": pokemon, "evolutions": evolutions}, indent=2) + "\n",
        encoding="utf-8",
    )
    print(f"wrote {OUT} ({len(pokemon)} pokemon, {len(evolutions)} evolutions)")


def species_id(url: str) -> int:
    return int(url.rstrip("/").split("/")[-1])


def walk_chain(node: dict, evolutions: list[dict], seen: set[tuple[int, int]]) -> None:
    from_id = species_id(node["species"]["url"])
    if from_id > 151:
        return
    for link in node.get("evolves_to", []):
        to_id = species_id(link["species"]["url"])
        if to_id <= 151 and (from_id, to_id) not in seen:
            details = link["evolution_details"][0] if link["evolution_details"] else {}
            trigger = (details.get("trigger") or {}).get("name") or "unknown"
            evolutions.append(
                {
                    "from": from_id,
                    "to": to_id,
                    "trigger": trigger,
                    "minLevel": details.get("min_level"),
                }
            )
            seen.add((from_id, to_id))
        walk_chain(link, evolutions, seen)


if __name__ == "__main__":
    main()
