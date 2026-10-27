# PokédexAPI

A production-style ASP.NET Core backend that serves Pokémon data, lets trainers sign up, catch Pokémon and build teams, and ships to the cloud in a container.

I'm building it chapter by chapter, following the [roadmap.sh backend path](https://roadmap.sh/backend). Each chapter adds one layer (data, API design, testing, auth, caching, messaging, security, deployment) to the same codebase.

> **Status:** Chapter 0 in progress: a console Pokédex in C#.

## Tech stack

- C# on .NET 10 (LTS), ASP.NET Core
- Entity Framework Core + PostgreSQL
- Redis, RabbitMQ
- xUnit, Testcontainers
- Docker, GitHub Actions
- Data from [PokéAPI](https://pokeapi.co/)

## Roadmap

| Chapter | What it adds | Status |
| --- | --- | --- |
| 0 | Foundations: console Pokédex | 🟡 In progress |
| 1 | First API: in-memory REST endpoints | ⬜ |
| 2 | PostgreSQL + EF Core | ⬜ |
| 3 | Search, paging, OpenAPI, versioning | ⬜ |
| 4 | Layered architecture + automated tests | ⬜ |
| 5 | Trainers: JWT auth and roles | ⬜ |
| 6 | PokéAPI importer + Redis caching | ⬜ |
| 7 | Background jobs, messaging, SignalR | ⬜ |
| 8 | Security, observability, rate limiting | ⬜ |
| 9 | Docker, CI/CD, cloud deployment | ⬜ |
| 10 | Capstone: Battle Service (gRPC) | ⬜ |

Each chapter is built on its own branch (`ch0-foundations`, `ch1-first-api`, …) and merged through a pull request.

## Getting started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Git

### Run it

```bash
git clone https://github.com/Seadimo12/PokedexAPI.git
cd PokedexAPI
dotnet run --project src/Pokedex.Console
```

### Commands (Chapter 0)

```text
find pikachu                 Show one Pokémon's details
type fire                    List all Pokémon of a type
top attack 10                Top 10 Pokémon by a stat
compare bulbasaur charmander Compare two Pokémon side by side
```

## Project structure

```text
PokedexAPI/
├── src/
│   └── Pokedex.Console/     Chapter 0 console app
├── data/
│   └── pokemon.json         First 151 Pokémon
├── notes/                   Learning notes (HTTP, etc.)
└── README.md
```

This section will grow as the solution is split into layers in Chapter 4.

## What I'm learning

- How HTTP works end to end (see [`notes/http.md`](notes/http.md))
- C# records, enums, LINQ and pattern matching
- Git branching and pull-request workflow

## Author

**Seadimo Bugqwangu**, Junior Software Engineer

- GitHub: [@Seadimo12](https://github.com/Seadimo12)
- LinkedIn: [seadimo-bugqwangu](https://www.linkedin.com/in/seadimo-bugqwangu-22609426b)

Pokémon and Pokémon names are trademarks of Nintendo, Game Freak and The Pokémon Company. This is a non-commercial learning project. Data comes from PokéAPI.
