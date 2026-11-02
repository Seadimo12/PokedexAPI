# PokédexAPI

A production-style ASP.NET Core backend that serves Pokémon data, lets trainers sign up, catch Pokémon and build teams, and ships to the cloud in a container.

I'm building it chapter by chapter, following the [roadmap.sh backend path](https://roadmap.sh/backend). Each chapter adds one layer (data, API design, testing, auth, caching, messaging, security, deployment) to the same codebase.

> **Status:** Chapter 2 in progress: PostgreSQL + EF Core behind the Chapter 1 endpoints.

## Tech stack

- C# on .NET 10 (LTS), ASP.NET Core
- Entity Framework Core + PostgreSQL
- Redis, RabbitMQ (later chapters)
- xUnit, Testcontainers (later chapters)
- Docker, GitHub Actions
- Data from [PokéAPI](https://pokeapi.co/)

## Roadmap

| Chapter | What it adds | Status |
| --- | --- | --- |
| 0 | Foundations: console Pokédex | ✅ |
| 1 | First API: in-memory REST endpoints | ✅ (now backed by Postgres) |
| 2 | PostgreSQL + EF Core | 🟡 In progress |
| 3 | Search, paging, OpenAPI, versioning | ⬜ |
| 4 | Layered architecture + automated tests | ⬜ |
| 5 | Trainers: JWT auth and roles | ⬜ |
| 6 | PokéAPI importer + Redis caching | ⬜ |
| 7 | Background jobs, messaging, SignalR | ⬜ |
| 8 | Security, observability, rate limiting | ⬜ |
| 9 | Docker, CI/CD, cloud deployment | ⬜ |
| 10 | Capstone: Battle Service (gRPC) | ⬜ |

Each chapter is built on its own branch (`ch0-foundations`, `ch1-first-api`, `ch2-data`, …) and merged through a pull request.

## Getting started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Git
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (for PostgreSQL)

### Console (Chapter 0)

```bash
dotnet run --project src/Pokedex.Console -- find pikachu
dotnet run --project src/Pokedex.Console -- type fire
dotnet run --project src/Pokedex.Console -- top attack 10
dotnet run --project src/Pokedex.Console -- compare bulbasaur charmander
```

### API (Chapters 1–2)

```bash
docker compose up -d
dotnet run --project src/Pokedex.Api
```

Startup applies EF Core migrations and seeds the first 151 Pokémon from `data/pokemon.json`.

| Method | Route | Result |
| --- | --- | --- |
| GET | `/pokemon` | All Pokémon |
| GET | `/pokemon/{id}` | One Pokémon or 404 |
| GET | `/pokemon/by-name/{name}` | One Pokémon or 404 |
| GET | `/types/{type}/pokemon` | Pokémon of a type |
| GET | `/pokemon/{id}/evolutions` | Full evolution chain or 404 |
| POST | `/pokemon` | 201 + `Location`, or 409 on unique conflict |
| PUT | `/pokemon/{id}` | 204, 404, or 409 |
| DELETE | `/pokemon/{id}` | 204 or 404 |

Exercise every route from `Pokedex.http`. In Development, EF logs the generated SQL so you can confirm list endpoints use a projection (no N+1) instead of a query per row.

The evolution-chain query in raw SQL lives in [`notes/evolution-chain.sql`](notes/evolution-chain.sql).

Refresh the seed file from PokéAPI with `python tools/fetch-gen1.py`.

## Project structure

```text
PokedexAPI/
├── src/
│   ├── Pokedex.Console/     Chapter 0 console app
│   └── Pokedex.Api/         Minimal APIs + EF Core
├── data/
│   └── pokemon.json         First 151 Pokémon + evolution links
├── notes/                   HTTP notes and the evolution SQL
├── docker-compose.yml       Local PostgreSQL
├── Pokedex.http             Request file for every endpoint
└── README.md
```

This section will grow as the solution is split into layers in Chapter 4.

## What I'm learning

- How HTTP works end to end (see [`notes/http.md`](notes/http.md))
- C# records, enums, LINQ and pattern matching
- Relational modelling, EF Core migrations, and avoiding N+1 queries
- Git branching and pull-request workflow

## Author

**Seadimo Bugqwangu**, Junior Software Engineer

- GitHub: [@Seadimo12](https://github.com/Seadimo12)
- LinkedIn: [seadimo-bugqwangu](https://www.linkedin.com/in/seadimo-bugqwangu-22609426b)

Pokémon and Pokémon names are trademarks of Nintendo, Game Freak and The Pokémon Company. This is a non-commercial learning project. Data comes from PokéAPI.
