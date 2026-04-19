# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build the solution
dotnet build

# Run the API (from repo root)
dotnet run --project WebAPI/WebAPI.csproj

# Apply EF Core migrations
dotnet ef database update --project Infrastructure/Infrastructure.csproj --startup-project WebAPI/WebAPI.csproj

# Add a new migration
dotnet ef migrations add <MigrationName> --project Infrastructure/Infrastructure.csproj --startup-project WebAPI/WebAPI.csproj

# Run tests
dotnet test Test/Test.csproj

# Run tests with coverage
dotnet test Test/Test.csproj --collect:"XPlat Code Coverage"
```

Swagger UI is available at `https://localhost:<port>/swagger/index.html` when running in Development.

## Architecture

This is an ASP.NET Core 9.0 Web API following **Clean Architecture**. Dependencies flow inward: `WebAPI → Application → Infrastructure → Domain`. The `Utilities` project is shared by all layers.

### Layer responsibilities

| Project | Role |
|---|---|
| `Domain` | Entities only — `BaseEntity` (Id, Name, CreatedAt, UpdatedAt, DeletedAt), `Museum`, `Article` |
| `Infrastructure` | EF Core `MuseumsDbContext`, generic + specialized repositories, Unit of Work, DI registration |
| `Application` | Service interfaces and implementations, AutoMapper profiles, FluentValidation validators, request/response ViewModels, `BaseResponse<T>` wrapper |
| `WebAPI` | Controllers, `Program.cs`, appsettings, Swagger config |
| `Utilities` | `Themes` enum (Art/NaturalSciences/History), `ReplyMessages` string constants |

### Key patterns

**Repository + Unit of Work**: `IGenericRepository<T>` handles CRUD. `IUnitOfWork` exposes `MuseumRepository` and `ArticleRepository`. All data access goes through `IUnitOfWork`.

**Soft delete**: Every entity has `DeletedAt`. `Remove` endpoints set this field; `Delete` endpoints physically remove rows. Repository queries must filter `DeletedAt == null` for active records.

**Standard response**: All API responses are wrapped in `BaseResponse<T>` (`IsSuccess`, `Message`, `Data`, `Errors`). Use `ReplyMessages` constants for message strings.

**DI registration**: Each layer registers its own services via `InjectionExtensions.AddXxx(IServiceCollection)` extension methods called from `Program.cs`. Add new services there.

**Validation**: FluentValidation validators live in `Application/Validators/`. They are auto-registered in `Application`'s DI extension. Validation errors surface through `BaseResponse.Errors`.

**AutoMapper**: Profiles are in `Application/Mappers/`. Mapping goes Entity ↔ ViewModel. Register new profiles in the same folder — they are picked up automatically via assembly scan.

### Data model

```
Museum (1) ──── (Many) Article
  Id, Name, Theme (int enum), CreatedAt, UpdatedAt, DeletedAt
                     Article
                       Id, Name, IsDamaged (bool?), IdMuseum (FK), CreatedAt, UpdatedAt, DeletedAt
```

### Database

SQL Server, local instance. Connection string key: `MuseumsDBConnectionString` in `appsettings.json`. Uses Windows authentication (`Trusted_Connection=True`). Collation: `Modern_Spanish_CI_AS`. Migrations assembly is `Infrastructure`.
