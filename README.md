# Support Ticket Management System

A full-stack support ticket management system built with **Blazor WebAssembly**, **ASP.NET Core Web API (.NET 9)**, and **SQL Server LocalDB**. Part of the .NET AI Capability Exercise — demonstrates Core ticket lifecycle management with a **state-machine-enforced** status workflow, comments, search/filter, and integration test coverage.

## Features (Core)

- Create, read, and update support tickets
- Add comments to tickets
- **State machine** status transitions (Open → InProgress → Resolved → Closed, plus Cancelled paths)
- Search tickets by keyword (title/description)
- Filter tickets by status
- Persistent storage in SQL Server (verified across API restarts)
- Server-side validation and structured API error responses
- Blazor UI with loading states, empty states, and toast notifications

## Technology Stack

| Layer | Technology |
|-------|------------|
| Frontend | Blazor WebAssembly (.NET 9) |
| Backend | ASP.NET Core Web API (.NET 9) |
| Database | SQL Server LocalDB + EF Core 9 |
| Testing | xUnit, WebApplicationFactory, in-memory SQLite |
| API docs | Swashbuckle (Swagger UI, Development only) |

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- **SQL Server LocalDB** (included with Visual Studio or SQL Server Express)
  - Verify: `SqlLocalDB.exe info` (Windows)
- **EF Core CLI tools** (global):
  ```bash
  dotnet tool install --global dotnet-ef
  ```
- IDE: Visual Studio 2022, VS Code, or Cursor

## Project Structure

```
ai-practical-assessment/
├── src/
│   ├── TicketSystem.Api/       # ASP.NET Core Web API
│   └── TicketSystem.Blazor/    # Blazor WebAssembly frontend
├── tests/
│   └── TicketSystem.Tests/     # Integration tests (xUnit)
├── database/                     # Setup notes and schema docs
├── ai-prompts/                   # Prompt history by lifecycle phase
├── TicketSystem.slnx             # Solution file
└── [lifecycle documentation]     # design-notes.md, api-contract.md, etc.
```

## Database Setup

### 1. Connection string

Default in `src/TicketSystem.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TicketSystemDb;Trusted_Connection=true;TrustServerCertificate=true"
  }
}
```

For SQL authentication or remote servers, use [User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) — **never commit passwords**.

### 2. Apply migrations

From the repository root:

```bash
cd src/TicketSystem.Api
dotnet ef database update
```

This creates `TicketSystemDb` and seeds **5 users** (Admin, Agents, Users). See `database/setup-notes.md` for persistence verification steps.

### 3. EF Core migration commands (reference)

```bash
# Add a new migration (after model changes)
dotnet ef migrations add MigrationName

# Apply migrations
dotnet ef database update

# List migrations
dotnet ef migrations list
```

Initial migration: `20260714093803_InitialCreate`

## Running the Application

Use the **`http` launch profile** for both projects during local development to avoid mixed-content and CORS issues between Blazor and the API.

### Terminal 1 — API

```bash
cd src/TicketSystem.Api
dotnet run --launch-profile http
```

| Profile | URL |
|---------|-----|
| HTTP (recommended) | http://localhost:5041 |
| HTTPS | https://localhost:7090 |

**Swagger UI (Development):** http://localhost:5041/swagger

### Terminal 2 — Blazor frontend

```bash
cd src/TicketSystem.Blazor
dotnet run --launch-profile http
```

| Profile | URL |
|---------|-----|
| HTTP (recommended) | http://localhost:5036 |
| HTTPS | https://localhost:7155 |

Open **http://localhost:5036** in your browser. The app redirects `/` to `/tickets`.

### API base URL (Blazor)

Configured in `src/TicketSystem.Blazor/wwwroot/appsettings.json`:

```json
{
  "ApiBaseUrl": "http://localhost:5041"
}
```

Update this if your API runs on a different port.

## Running Tests

```bash
cd tests/TicketSystem.Tests
dotnet test
```

**28 integration tests** cover:
- State machine transitions (5 valid, 12+ invalid cases)
- Ticket CRUD
- Comment creation
- Search and filter

Tests use `WebApplicationFactory` with in-memory SQLite (`ASPNETCORE_ENVIRONMENT=Testing`). No SQL Server required for test execution.

Build the full solution (optional):

```bash
dotnet build TicketSystem.slnx
```

## Seeded Users

| Id | Name | Email | Role |
|----|------|-------|------|
| 1 | Admin User | admin@ticketsystem.com | Admin |
| 2 | Support Agent | agent@ticketsystem.com | Agent |
| 3 | Regular User | user@ticketsystem.com | User |
| 4 | Jane Smith | jane.smith@ticketsystem.com | Agent |
| 5 | Bob Johnson | bob.johnson@ticketsystem.com | User |

Use these IDs when creating tickets or comments via the API or UI.

## State Machine (Signature Feature)

**Valid transitions:**
- Open → InProgress, Cancelled
- InProgress → Resolved, Cancelled
- Resolved → Closed

**Terminal states:** Closed, Cancelled (no further transitions)

Invalid transitions return `400 Bad Request` with structured error details. See `data-model.md` and `api-contract.md`.

## Known Limitations (Core Scope)

- **No authentication** — all endpoints and UI routes are open (Stretch per spec)
- **No pagination** — all tickets loaded in list view
- **Client-selected `CreatedById`** — identity is chosen from dropdown, not from login
- LocalDB / HTTP dev configuration — not production-hardened

See `code-review-notes.md` and `review-fixes.md` for review findings and targeted fixes.

## Documentation

| Document | Description |
|----------|-------------|
| `spec.md` | Exercise specification |
| `implementation-plan.md` | 11-phase implementation plan |
| `api-contract.md` | REST API reference |
| `data-model.md` | Entities, relationships, state machine |
| `design-notes.md` | Architecture and design decisions |
| `test-strategy.md` | Testing approach |
| `code-review-notes.md` | Phase 9 review findings |
| `review-fixes.md` | Accepted/deferred review fixes |
| `ai-prompts/` | Prompt history by lifecycle phase |
| `database/setup-notes.md` | DB setup and persistence verification |

**Candidate-authored (not AI-generated):** `reflection.md`, `tool-workflow.md`

## Troubleshooting

| Issue | Fix |
|-------|-----|
| Ticket list stuck on "Loading..." | Ensure API is running on port 5041; use `http` profiles for both apps |
| CORS errors in browser | API `Program.cs` allows `http://localhost:5036`; restart API after config changes |
| `dotnet ef` not found | Install global tool: `dotnet tool install --global dotnet-ef` |
| Build file-lock error | Stop running `TicketSystem.Api` process, then rebuild |
| Migration fails | Confirm LocalDB is installed and connection string is correct |

## License

Internal .NET AI Capability Exercise submission.
