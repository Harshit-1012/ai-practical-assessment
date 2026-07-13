# Support Ticket Management System

A full-stack support ticket management system built with Blazor WebAssembly, ASP.NET Core Web API (.NET 9), and SQL Server.

## Prerequisites

- .NET 9 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code with C# extension

## Setup Instructions

### Database Setup

1. Update the connection string in `src/TicketSystem.Api/appsettings.json`
2. Run database migrations:
   ```bash
   cd src/TicketSystem.Api
   dotnet ef database update
   ```

### Running the Application

1. Start the API:
   ```bash
   cd src/TicketSystem.Api
   dotnet run
   ```

2. Start the Blazor frontend:
   ```bash
   cd src/TicketSystem.Blazor
   dotnet run
   ```

3. Navigate to the Blazor app URL (typically https://localhost:7xxx)

### Running Tests

```bash
cd tests/TicketSystem.Tests
dotnet test
```

## Features

- Create, view, update, and delete support tickets
- Add comments to tickets
- State machine-enforced ticket status transitions
- Search tickets by keyword
- Filter tickets by status
- Persistent data storage

## Project Structure

- `src/TicketSystem.Api` - ASP.NET Core Web API backend
- `src/TicketSystem.Blazor` - Blazor WebAssembly frontend
- `tests/TicketSystem.Tests` - xUnit integration and unit tests
- `database/` - Database schema, migrations, and seed data

## Technology Stack

- **Frontend:** Blazor WebAssembly
- **Backend:** ASP.NET Core Web API (.NET 9)
- **Database:** SQL Server with Entity Framework Core
- **Testing:** xUnit

## Documentation

See the following files for detailed information:

- `implementation-plan.md` - Overall implementation approach
- `design-notes.md` - Architecture and design decisions
- `api-contract.md` - API endpoint specifications
- `test-strategy.md` - Testing approach
- `reflection.md` - Project reflection and AI usage notes
