# 🏛️ Museum API

![.NET 9](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)
![xUnit](https://img.shields.io/badge/Tests-xUnit%2044%20passing-brightgreen?logo=xunit)
![Architecture](https://img.shields.io/badge/Architecture-Clean%20Architecture-blue)
![EF Core](https://img.shields.io/badge/ORM-EF%20Core%209-purple)
![SQL Server](https://img.shields.io/badge/DB-SQL%20Server-CC2927?logo=microsoftsqlserver)

A RESTful Web API for managing museums and their articles, built as a technical challenge for **Xtra Travel**. It follows Clean Architecture principles, enforces soft-delete patterns, and ships with a full test suite (44 tests, InMemory DB).

---

## 🗂️ Table of Contents

- [Architecture](#-architecture)
- [Tech Stack](#-tech-stack)
- [API Endpoints](#-api-endpoints)
- [Data Model](#-data-model)
- [Prerequisites & Installation](#-prerequisites--installation)
- [Running the Tests](#-running-the-tests)
- [Known Technical Debt](#-known-technical-debt)

---

## 🏗️ Architecture

This project follows **Clean Architecture**. Dependencies always point inward — outer layers depend on inner ones, never the reverse.

```
┌──────────────────────────────────────────────────┐
│                    WebAPI                        │  ← Controllers, Program.cs, Swagger
│  ┌──────────────────────────────────────────┐  │
│  │               Application                 │  │  ← Services, Validators, AutoMapper, ViewModels
│  │  ┌────────────────────────────────────┐ │  │
│  │  │           Infrastructure             │ │  │  ← EF Core, Repositories, Unit of Work
│  │  │  ┌────────────────────────────────┐ │ │  │
│  │  │  │           Domain               │ │ │  │  ← Entities only (no dependencies)
│  │  │  └────────────────────────────────┘ │ │  │
│  │  └────────────────────────────────────┘ │  │
│  └──────────────────────────────────────────┘  │
└──────────────────────────────────────────────────┘
              ↕ (all layers)
         Utilities  (ReplyMessages, Themes enum)
```

### Layer responsibilities

| Project | Role |
|---|---|
| `Domain` | Entities: `BaseEntity` (Id, Name, CreatedAt, UpdatedAt, DeletedAt), `Museum`, `Article` |
| `Infrastructure` | `MuseumsDbContext`, generic + specialised repositories, Unit of Work, DI registration |
| `Application` | Service interfaces & implementations, AutoMapper profiles, FluentValidation validators, request/response ViewModels, `BaseResponse<T>` wrapper |
| `WebAPI` | Controllers, `Program.cs`, appsettings, Swagger config |
| `Utilities` | `Themes` enum (Art / NaturalSciences / History), `ReplyMessages` string constants |

---

## 🛠️ Tech Stack

| Concern | Technology |
|---|---|
| Framework | ASP.NET Core 9.0 |
| Language | C# 13 |
| ORM | Entity Framework Core 9 |
| Database | SQL Server (Windows auth) |
| Validation | FluentValidation 11 |
| Object mapping | AutoMapper 13 |
| API docs | Swashbuckle / Swagger 10 |
| Testing | xUnit + EF Core InMemory |
| Architecture | Clean Architecture + Repository + Unit of Work |

---

## 📡 API Endpoints

All responses are wrapped in `BaseResponse<T>`:

```json
{"isSuccess": true, "message": "Query successful.", "data": {}, "errors": null}
```

### Museum — `api/Museum`

| Method | Route | Description | Success | Error codes |
|---|---|---|---|---|
| `GET` | `/api/Museum/All` | List all active museums | 200 | — |
| `GET` | `/api/Museum/Select` | List museums (id + name only, for dropdowns) | 200 | — |
| `GET` | `/api/Museum/{museumId}` | Get museum by ID | 200 | 404 |
| `GET` | `/api/Museum/ArticlesByMuseum/{museumId}` | List active articles belonging to a museum | 200 | 404 |
| `GET` | `/api/Museum/GetMuseumsByTheme/{theme}` | Filter museums by theme (1=Art, 2=NaturalSciences, 3=History) | 200 | 400 |
| `POST` | `/api/Museum/Register` | Create a new museum | 201 | 400 |
| `PUT` | `/api/Museum/Edit/{museumId}` | Update museum name / theme | 200 | 400, 404 |
| `PUT` | `/api/Museum/Remove/{museumId}` | Soft-delete museum (cascades to articles) | 200 | 400, 404 |
| `DELETE` | `/api/Museum/Delete/{museumId}` | Hard-delete museum (blocked if it has active articles) | 200 | 400, 404 |

### Article — `api/Article`

| Method | Route | Description | Success | Error codes |
|---|---|---|---|---|
| `GET` | `/api/Article/All` | List all active articles | 200 | — |
| `GET` | `/api/Article/{articleId}` | Get article by ID | 200 | 404 |
| `POST` | `/api/Article/Register` | Create a single article | 201 | 400 |
| `POST` | `/api/Article/BulkRegister` | Create multiple articles atomically (all-or-nothing) | 201 | 400 |
| `PUT` | `/api/Article/Edit/{articleId}` | Update article fields | 200 | 400, 404 |
| `PUT` | `/api/Article/MarkDamaged/{articleId}` | Set `IsDamaged = true` on an article | 200 | 400, 404 |
| `PUT` | `/api/Article/Relocate/{articleId}` | Move article to a different museum | 200 | 400, 404 |
| `PUT` | `/api/Article/Remove/{articleId}` | Soft-delete article | 200 | 400, 404 |
| `DELETE` | `/api/Article/Delete/{articleId}` | Hard-delete article | 200 | 400, 404 |

---

## 🗄️ Data Model

```
┌──────────────────────────────────┐
│             Museum               │
├──────────────────────────────────┤
│ Id          int  (PK)            │
│ Name        string               │
│ Theme       int  (1/2/3)         │
│ CreatedAt   DateTime             │
│ UpdatedAt   DateTime?            │
│ DeletedAt   DateTime?  <- soft   │
└────────────────┬─────────────────┘
                 │ 1
                 │
                 │ many
┌────────────────▼─────────────────┐
│             Article              │
├──────────────────────────────────┤
│ Id          int  (PK)            │
│ Name        string               │
│ IsDamaged   bool?                │
│ IdMuseum    int  (FK -> Museum)  │
│ CreatedAt   DateTime             │
│ UpdatedAt   DateTime?            │
│ DeletedAt   DateTime?  <- soft   │
└──────────────────────────────────┘
```

**Theme enum**

| Value | Name |
|---|---|
| 1 | Art |
| 2 | Natural Sciences |
| 3 | History |

**Soft delete vs hard delete**

| Operation | Behaviour |
|---|---|
| `Remove` (PUT) | Sets `DeletedAt`; record stays in DB and can be recovered |
| `Delete` (DELETE) | Physically removes the row |
| `RemoveMuseum` | Cascades soft-delete to all active articles first |
| `DeleteMuseum` | Blocked (400) if the museum has active articles |

---

## 🚀 Prerequisites & Installation

**Prerequisites**

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9)
- SQL Server (local instance with Windows authentication)
- EF Core CLI tools: `dotnet tool install --global dotnet-ef`

**Steps**

```bash
# 1. Clone the repository
git clone https://github.com/nmino1984/Museum.git
cd Museum

# 2. Set the connection string
#    Open WebAPI/appsettings.json and update:
#    "MuseumsDBConnectionString": "Server=YOUR_SERVER;Database=MuseumsDB;Trusted_Connection=True;TrustServerCertificate=True"

# 3. Apply EF Core migrations
dotnet ef database update \
  --project Infrastructure/Infrastructure.csproj \
  --startup-project WebAPI/API.csproj

# 4. Run the API
dotnet run --project WebAPI/API.csproj
```

Swagger UI will be available at `https://localhost:<port>/swagger/index.html`.

---

## 🧪 Running the Tests

```bash
# Run all tests
dotnet test Test/Test.csproj

# Run with coverage report
dotnet test Test/Test.csproj --collect:"XPlat Code Coverage"
```

**What the 44 tests cover**

| Area | Scenarios |
|---|---|
| Museum CRUD | Create, read, update, soft-delete, hard-delete |
| Museum queries | List all, get by ID, filter by theme, list articles by museum |
| Museum business rules | Hard-delete blocked when active articles exist; soft-delete cascades |
| Article CRUD | Create, read, update, soft-delete, hard-delete |
| Article operations | `MarkDamaged`, `RelocateArticle`, `BulkRegister` (atomic) |
| Validation | Missing name, invalid theme, invalid `IdMuseum` |

All tests use **EF Core InMemory** — no SQL Server instance required to run the suite.

---

## ⚠️ Known Technical Debt

| Item | Detail |
|---|---|
| Manual navigation properties | `Article.Museum` and `Museum.Articles` are not declared as EF navigation properties. `GetArticlesByMuseum` loads museum + articles in two separate queries and joins them in memory. |
| `ListAll` unused in repository | `IGenericRepository<T>` exposes a `ListAll()` method, but services call specialised repository methods that apply soft-delete filtering. The generic version is never called from a controller. |
| No authentication / authorisation | All endpoints are publicly accessible. A real deployment would require JWT / API key middleware. |
| CORS placeholder | `localhost:4200` and `localhost:3000` are hardcoded as allowed origins in `Program.cs`. |

---

🇪🇸 [Ver versión en español](README.es.md)
