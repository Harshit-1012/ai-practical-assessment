# Design Notes

## Architecture Overview

### System Architecture

The application is a **client-server** system with clear separation between presentation, API, business logic, and persistence:

```
┌─────────────────────────────────────────────────────────────┐
│  Blazor WebAssembly (TicketSystem.Blazor)                   │
│  Pages + Components → Client Services → HttpClient          │
│  http://localhost:5036                                      │
└──────────────────────────┬──────────────────────────────────┘
                           │ REST / JSON (CORS)
┌──────────────────────────▼──────────────────────────────────┐
│  ASP.NET Core Web API (TicketSystem.Api)                    │
│  Controllers → Services → State Machine → AppDbContext      │
│  http://localhost:5041                                      │
└──────────────────────────┬──────────────────────────────────┘
                           │ EF Core
┌──────────────────────────▼──────────────────────────────────┐
│  SQL Server LocalDB — TicketSystemDb                        │
└─────────────────────────────────────────────────────────────┘
```

**Solution structure:**

```
src/
  TicketSystem.Api/       — Backend API, entities, EF Core, state machine
  TicketSystem.Blazor/    — Blazor WASM frontend
tests/
  TicketSystem.Tests/     — Integration tests (WebApplicationFactory + SQLite)
```

### Technology Decisions

| Layer | Choice | Rationale |
|-------|--------|-----------|
| Frontend | Blazor WebAssembly (.NET 9) | Full-stack C#, shared mental model, component-based UI without a separate JS framework |
| Backend | ASP.NET Core Web API (.NET 9) | Strong typing, async I/O, mature DI and middleware pipeline |
| ORM | Entity Framework Core 9 | Code-first migrations, Fluent API, LINQ queries |
| Database | SQL Server LocalDB | Relational model fits tickets/users/comments; persistence verified across API restarts |
| API docs | Swashbuckle (Swagger) | Dev-only interactive docs at `/swagger` |
| Tests | xUnit + WebApplicationFactory | End-to-end API tests with in-memory SQLite |

**Trade-offs accepted:**
- Blazor WASM has a larger initial download than a minimal SPA; acceptable for an internal tool demo.
- No repository abstraction — `AppDbContext` is used directly in services to keep the Core scope lean.
- Status/priority/role stored as strings in DB rather than DB enums for simplicity and readability.

---

## Frontend Design

### Component Structure

**Layout:**
- `MainLayout.razor` — app shell with sidebar + top bar
- `NavMenu.razor` — navigation links (`/tickets`, `/tickets/create`)

**Pages (`Pages/Tickets/`):**

| Page | Route | Responsibility |
|------|-------|----------------|
| `Index.razor` | `/` | Redirects to `/tickets` |
| `TicketList.razor` | `/tickets` | Card grid, search/filter, loading/empty states |
| `TicketDetail.razor` | `/tickets/{id}` | Ticket metadata, status transitions, comments |
| `CreateTicket.razor` | `/tickets/create` | New ticket form |
| `EditTicket.razor` | `/tickets/{id}/edit` | Edit title, description, priority, assignee |

**Reusable components (`Components/`):**

| Component | Purpose |
|-----------|---------|
| `StatusBadge` | Colored status label |
| `PriorityBadge` | Colored priority label |
| `SearchFilter` | Keyword + status filter controls |
| `CommentList` | Renders comment thread |
| `AddComment` | Comment submission form |
| `StatusTransitionActions` | Transition buttons with disabled state + tooltip |
| `LoadingSpinner` | Async loading indicator |
| `EmptyState` | No-results placeholder |
| `ToastContainer` | Toast notification host |

**Styling:** Custom indigo/slate theme in `wwwroot/css/app.css` and scoped `.razor.css` files. No Bootstrap dependency.

### State Management

- **Page-local state:** Loading flags, error messages, and bound form fields live in each page's `@code` block.
- **No global store:** No Flux/Redux pattern; state is fetched on page init and refreshed after mutations.
- **Shared services via DI:** API calls, display formatting, workflow rules, and notifications are injected with `@inject`.
- **Notifications:** `NotificationService` exposes an event (`OnChange`) consumed by `ToastContainer` for toast UI.

### Client Services (business logic layer)

Components delegate to services; they do not embed transition rules or formatting logic.

| Service | Responsibility |
|---------|----------------|
| `TicketApiService` | HTTP CRUD + status change for tickets |
| `CommentApiService` | HTTP for comments |
| `UserApiService` | HTTP for user list (assignment dropdown) |
| `ApiClientHelper` | Centralized GET/POST/PUT, JSON deserialization, `ApiClientException` on errors |
| `TicketWorkflowService` | UI-only mirror of state machine — enabled/disabled transitions, labels, tooltips |
| `TicketDisplayService` | Badge CSS classes, labels, relative dates, filter/priority options |
| `NotificationService` | Toast queue (success, error, info) |

`TicketWorkflowService` duplicates transition rules from the backend **for presentation only**. The API remains authoritative; invalid UI actions that slip through still return `400`.

### API Communication

- `HttpClient` registered in `Program.cs` with `BaseAddress` from `wwwroot/appsettings.json` (`http://localhost:5041`).
- 30-second timeout configured on the client.
- Errors parsed into `ApiErrorResponse` by `ApiClientHelper` and surfaced as `ApiClientException`.
- Pages catch exceptions and set `errorMessage` + call `NotificationService.ShowError`.

### UI/UX Patterns

- **Loading:** `LoadingSpinner` while async operations run.
- **Empty states:** `EmptyState` when ticket list has no results.
- **Errors:** Inline `alert-error` divs on pages + toast notifications for mutations.
- **Status transitions:** All five target statuses shown as buttons; invalid ones are disabled with `title` tooltip explaining why.
- **Card layout:** Ticket list uses hoverable cards with status/priority badges.

---

## Backend Design

### Layering Architecture

```
Controllers (HTTP, routing, model binding)
    ↓
Services (business logic, validation, orchestration)
    ↓
TicketStateMachine (status transition rules)
    ↓
AppDbContext (EF Core data access)
    ↓
SQL Server
```

### Controller Layer

Three controllers, all `[ApiController]` with `[Route("api/[controller]")]` except comments:

| Controller | Routes |
|------------|--------|
| `TicketsController` | `/api/tickets`, `/api/tickets/{id}`, `/api/tickets/{id}/status` |
| `CommentsController` | `/api/tickets/{ticketId}/comments` |
| `UsersController` | `/api/users`, `/api/users/{id}` |

Controllers are thin: validate via model binding, delegate to services, return appropriate HTTP status codes (`200`, `201`, `404`, etc.).

### Service Layer

| Service | Interface | Responsibilities |
|---------|-----------|------------------|
| `TicketService` | `ITicketService` | CRUD, search/filter, status changes, FK validation, DTO mapping |
| `CommentService` | `ICommentService` | List/create comments, existence checks |
| `UserService` | `IUserService` | Read-only user queries |
| `TicketStateMachine` | `ITicketStateMachine` | Transition validation |

All services are **scoped** (per HTTP request). All I/O is async.

### State Machine Design

**Interface:**

```csharp
public interface ITicketStateMachine
{
    bool CanTransition(TicketStatus from, TicketStatus to);
    void ValidateTransition(TicketStatus from, TicketStatus to);
}
```

**Implementation:** Static `Dictionary<TicketStatus, TicketStatus[]>` in `TicketStateMachine`. `ValidateTransition` throws `InvalidTransitionException` with a human-readable message listing valid targets.

**Enforcement:** Only `TicketService.ChangeStatusAsync` calls the state machine. `UpdateTicketAsync` cannot change status.

**Extensibility:** Adding a new transition requires updating the dictionary, `TicketStatus` enum (if new status), frontend `TicketWorkflowService`, and integration tests.

### Data Access Layer

- Single `AppDbContext` with `DbSet<User>`, `DbSet<Ticket>`, `DbSet<Comment>`.
- Fluent API in `OnModelCreating` for lengths, relationships, delete behaviors, and user seed data.
- Queries use `Include`/`ThenInclude` for related user names on reads.
- List endpoint uses `AsNoTracking()` for read performance.
- Migrations managed with `dotnet ef migrations add`; initial migration `20260714093803_InitialCreate`.

**Testing environment:** When `ASPNETCORE_ENVIRONMENT=Testing`, `Program.cs` skips SQL Server registration; `CustomWebApplicationFactory` registers SQLite in-memory instead.

---

## Database Design

### Entity Relationships

```
User 1 ──→ * Ticket  (CreatedBy, required)
User 1 ──→ * Ticket  (AssignedTo, optional)
User 1 ──→ * Comment (CreatedBy, required)
Ticket 1 ──→ * Comment (cascade delete)
```

### Migration Strategy

1. Change entity or Fluent API configuration.
2. `dotnet ef migrations add <Name>` from `src/TicketSystem.Api`.
3. `dotnet ef database update` to apply to LocalDB.
4. Seed data for users is embedded in the initial migration via `HasData`.

---

## Validation Strategy

### Server-Side Validation

**Approach:** Data Annotations on DTOs + imperative checks in services.

| Layer | What it validates |
|-------|-------------------|
| Data Annotations | Required fields, max lengths on DTOs |
| `TicketService` | Priority enum, status enum (filter + change), user FK existence |
| `CommentService` | User FK existence, ticket existence |
| `TicketStateMachine` | Allowed status transitions |

Validation failures throw `BusinessValidationException` (field-level errors) or return `400` via `[ApiController]` model state factory.

### Client-Side Validation

- HTML5 `required` and basic form validation on create/edit/comment forms.
- No duplicate server rules on the client beyond workflow button enablement.
- API errors displayed to the user after submission.

---

## Error Handling Strategy

### Backend

**`ExceptionHandlingMiddleware`** catches unhandled exceptions and maps them to JSON:

| Exception | HTTP | Response shape |
|-----------|------|----------------|
| `NotFoundException` | 404 | `{ error, requestId }` |
| `InvalidTransitionException` | 400 | `{ error, currentStatus, requestedStatus, message, requestId }` |
| `BusinessValidationException` | 400 | `{ error, errors, requestId }` |
| All others | 500 | `{ error, requestId }` + server log |

**Model binding errors** use a custom `InvalidModelStateResponseFactory` returning `ApiError` with an `errors` dictionary.

**Pipeline order:** CORS → (HTTPS redirect in non-Development) → `ExceptionHandlingMiddleware` → controllers.

### Frontend

- `ApiClientHelper` throws `ApiClientException` with parsed message.
- Pages show inline error alerts and toasts via `NotificationService`.
- Ticket list shows a friendly message when the API is unreachable (connection/CORS issues).

---

## Testing Strategy

**Integration tests** (`tests/TicketSystem.Tests/IntegrationTests/`):

| Test class | Coverage |
|------------|----------|
| `StateMachineTransitionTests` | All 5 valid + 11 invalid transitions, persistence verification |
| `TicketCrudTests` | Create, read, update, list, 404 |
| `CommentIntegrationTests` | Create, list, 404 |
| `TicketSearchFilterTests` | Status filter, keyword search, combined filter, invalid status |

**Infrastructure:**
- `CustomWebApplicationFactory` — `WebApplicationFactory<Program>`, SQLite in-memory
- `TestDataSeeder` — DTO builders, direct DB seeding for specific statuses
- `IntegrationTestBase` — resets tickets/comments before each test
- **28 tests**, all passing

Unit tests for isolated state machine logic are not separately implemented; transition rules are fully covered by integration tests.

---

## Security Considerations

- Connection string in `appsettings.json` uses trusted LocalDB auth (no password in repo).
- No authentication/authorization in Core — all endpoints are open (stretch feature).
- EF Core parameterized queries prevent SQL injection.
- CORS restricted to known Blazor dev origins.
- HTTPS redirection enabled outside Development.
- Stack traces and internal details never returned in API error responses.

---

## Performance Considerations

- `AsNoTracking()` on read-only ticket list and comment list queries.
- Eager loading (`Include`) only where display names are needed.
- No pagination in Core — acceptable for demo scale; would be needed for production volume.
- Async/await throughout API and Blazor services.

---

## Future Enhancements

Stretch features not yet implemented:

- JWT authentication and role-based authorization
- User CRUD and admin management
- Pagination and advanced sorting on ticket list
- SignalR for real-time ticket updates
- Email notifications on assignment/status change
- Ticket audit/history log
- Docker + CI/CD pipeline
