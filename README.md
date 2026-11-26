# Library Minimal API

A small REST API for managing books, built with **ASP.NET Core Minimal APIs (.NET 10)**. This is a hands-on learning project covering HTTP endpoints, dependency injection, Entity Framework Core, SQLite, validation, API documentation, and integration testing.

## Features

- Create, retrieve, update, and delete books
- Search books by title and retrieve a single book by ISBN
- Persist data using **Entity Framework Core** and **SQLite**
- Apply model validation using data annotations and ASP.NET Core's validation services
- Explore the API through **OpenAPI** and **Swagger UI** in Development
- Exercise the HTTP endpoints through a separate **xUnit integration-test project**

## Tech stack

- **.NET 10 / C#** — ASP.NET Core Minimal APIs
- **Entity Framework Core 10** — persistence, model mapping, and migrations
- **SQLite** — local database
- **Microsoft.AspNetCore.OpenApi** — OpenAPI document generation
- **Swashbuckle Swagger UI** — interactive API documentation
- **xUnit + Microsoft.AspNetCore.Mvc.Testing** — integration testing with `WebApplicationFactory`

## Project structure

```text
libraryapi/
├── libraryapi.sln
├── src/
│   └── library.minimalapi/
│       ├── Contracts/          # IBookService and other interfaces
│       ├── Data/               # ApplicationDbContext
│       ├── Endpoints/          # BookEndpoints and BookHandlers
│       ├── Extensions/         # Startup and DI configuration
│       ├── Migrations/         # EF Core migrations
│       ├── Model/              # Book record and validation attributes
│       ├── Services/           # BookService
│       ├── Program.cs
│       ├── appsettings.json
│       └── library.minimalapi.csproj
├── tests/
│   └── library.minimalapi.tests.integration/
└── README.md
```

The application separates **route registration** (`BookEndpoints`), **HTTP request handling** (`BookHandlers`), and **data/business operations** (`BookService`).

## API endpoints

| Method | Route | Description |
| --- | --- | --- |
| `GET` | `/books` | Get all books |
| `GET` | `/book?isbn={isbn}` | Get a book by ISBN |
| `GET` | `/search?searchTerm={term}` | Search books by title |
| `POST` | `/book` | Create a book |
| `PUT` | `/book?isbn={isbn}` | Update a book |
| `DELETE` | `/book?isbn={isbn}` | Delete a book |

### Example book payload

```json
{
  "isbn": "9780132350884",
  "title": "Clean Code",
  "author": "Robert C. Martin",
  "shortDescription": "A book about writing maintainable code.",
  "pageCount": 464,
  "releaseDate": "2008-08-01"
}
```

`isbn` must contain **13 digits**. The model also validates title length, author length, description length, and page count. Dates are represented as `yyyy-MM-dd`.

### Example request

```http
GET http://localhost:5018/books
Accept: application/json
```

The API uses conventional HTTP responses such as `200 OK`, `201 Created`, `204 No Content`, `400 Bad Request`, and `404 Not Found`; duplicate ISBNs are handled as a conflict in the current implementation. Response details may evolve as the project is refined.

## Getting started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- EF Core command-line tools (`dotnet ef`) for migrations

### Run locally

From the repository root:

```powershell
dotnet restore
dotnet build
cd src/library.minimalapi
```

If `dotnet ef` is not installed, install it first:

```powershell
dotnet tool install --global dotnet-ef --version "10.*"
```

Apply the existing migrations to create the local SQLite schema:

```powershell
dotnet ef database update
```

Start the API:

```powershell
dotnet run
```

Use the local URL printed by the application. The sample HTTP requests use `http://localhost:5018`; your actual port may differ.

### Swagger / OpenAPI

When running in the **Development** environment:

- Swagger UI: `/swagger`
- OpenAPI JSON: `/openapi/v1.json`

The project uses ASP.NET Core's built-in OpenAPI document generator with Swagger UI as the interactive frontend.

## Integration tests

The `tests/library.minimalapi.tests.integration` project uses **xUnit** and **`WebApplicationFactory`** to send real HTTP requests through the application's routing and middleware. The tests exercise behaviors such as successful book creation, invalid input, and duplicate ISBN handling.

> **Database isolation:** Before running state-changing integration tests repeatedly, configure the test host to use its own SQLite database (or a correctly managed SQLite in-memory database). Tests should not write to the development `library.db` file.

Once the test database is isolated, run tests from the repository root:

```powershell
dotnet test
```

## Database notes

The default SQLite connection string is configured under `ConnectionStrings:DefaultConnection` in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=library.db"
  }
}
```

A relative SQLite path can resolve differently when the API runs directly versus under a test runner. To avoid surprises, resolve the database path against the application's **content root**, or configure an explicit absolute path for each environment.

EF Core migrations in `Migrations/` belong in version control. Local `library.db`, `library.db-wal`, `library.db-shm`, and generated `bin/` and `obj/` files do not.

## Project status

This is a **learning and portfolio project**, not a production-ready library management system. Current areas of focus include reliable HTTP error responses, test isolation, and integration-test coverage.
