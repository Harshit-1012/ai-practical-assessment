# Pull Request Description

## Summary

Implements the **Core** scope of the Support Ticket Management System: a Blazor WebAssembly frontend and ASP.NET Core Web API backend with SQL Server persistence, state-machine-enforced ticket lifecycle, comments, search/filter, 28 integration tests, and full lifecycle documentation for the .NET AI Capability Exercise.

**Project:** Support Ticket Management System  
**Type:** Feature (full-stack Core implementation)  
**Scope:** Frontend + Backend + Database + Integration tests + lifecycle artifacts

---

## Features Implemented

### Core Features ✓

- [x] Create, read, update tickets
- [x] Add comments to tickets
- [x] State machine-enforced status transitions (Open → InProgress → Resolved → Closed, plus Cancelled)
- [x] Search tickets by keyword
- [x] Filter tickets by status
- [x] Persistent data storage (SQL Server LocalDB)
- [x] Server-side input validation and structured error responses
- [x] Loading, empty, and error states in the UI

### Stretch Features

- [x] User authentication / JWT (demo sign-in by user select; mutations protected; reads anonymous)
- [ ] Role-based authorization
- [ ] Pagination and advanced filtering
- [ ] Real-time updates (SignalR)

---

## Technical Changes

### Backend (`TicketSystem.Api`)

**Controllers:**
- `TicketsController` — list (search/filter), get, create, update, change status
- `CommentsController` — list/create comments per ticket
- `UsersController` — read-only user list for assignment dropdowns

**Services:**
- `TicketService` — CRUD, search/filter, status changes via state machine
- `CommentService` — comment operations
- `UserService` — read-only user queries
- `TicketStateMachine` — **signature piece** enforcing 5 valid transitions only

**Infrastructure:**
- `ExceptionHandlingMiddleware` — structured `ApiError` JSON (400/404/500)
- EF Core `AppDbContext` with Fluent API + seed data (5 users)
- CORS policy for Blazor dev origins
- Swashbuckle Swagger UI at `/swagger` (Development)
- `Testing` environment gate for integration test DbContext swap

**Post-review fixes (Phase 9):**
- Whitespace-only input rejection (`NotWhitespaceAttribute` + service guards)
- Block updates to Closed/Cancelled tickets

### Frontend (`TicketSystem.Blazor`)

**Pages:** `TicketList`, `TicketDetail`, `CreateTicket`, `EditTicket`, `Index` (redirect)

**Components:** `StatusBadge`, `PriorityBadge`, `SearchFilter`, `CommentList`, `AddComment`, `StatusTransitionActions`, `LoadingSpinner`, `EmptyState`, `ToastContainer`

**Client services:** `TicketApiService`, `CommentApiService`, `UserApiService`, `TicketWorkflowService` (UI transition rules), `TicketDisplayService`, `NotificationService`, `ApiClientHelper`

**UI:** Custom indigo/slate theme (`wwwroot/css/app.css`) — no Bootstrap dependency in active UI

**Post-review fixes:**
- Comment author user dropdown (removed hardcoded user ID 3)
- `OnParametersSetAsync` on detail/edit pages for route parameter reload
- Sidebar nav styling (`NavMenu.razor.css` + `::deep` / global fallback)
- API connectivity fixes (CORS order, HttpClient timeout, SearchFilter binding)

### Database

**Entities:** `User`, `Ticket`, `Comment`  
**Migration:** `20260714093803_InitialCreate`  
**Persistence:** Verified — ticket survives API stop/restart (see `database/setup-notes.md`)

### Tests (`TicketSystem.Tests`)

**28 integration tests** via `WebApplicationFactory` + in-memory SQLite:
- `StateMachineTransitionTests` (16) — all mandatory valid/invalid transitions
- `TicketCrudTests` (5)
- `CommentIntegrationTests` (3)
- `TicketSearchFilterTests` (4)

Helpers: `CustomWebApplicationFactory`, `TestDataSeeder`, `HttpClientJsonExtensions`

---

## Database Changes

```sql
Users    (Id, Name, Email, Role)           -- seeded
Tickets  (Id, Title, Description, Priority, Status, AssignedToId, CreatedById, CreatedAt, UpdatedAt)
Comments (Id, TicketId, Message, CreatedById, CreatedAt)
```

**Setup:**
```bash
cd src/TicketSystem.Api
dotnet ef database update
```

---

## Testing Done

### Automated ✓

| Suite | Tests | Status |
|-------|-------|--------|
| State machine integration | 16 | Pass |
| Ticket CRUD | 5 | Pass |
| Comments | 3 | Pass |
| Search/filter | 4 | Pass |
| **Total** | **28** | **Pass** |

### Manual ✓

- Created/updated tickets through UI
- Exercised full valid status path (Open → InProgress → Resolved → Closed)
- Confirmed invalid transitions blocked (API 400 + disabled UI buttons)
- Added comments with selected author
- Search and status filter
- Data persistence after API restart
- Swagger manual API testing

---

## AI Usage Summary

**Primary tool:** Cursor AI with `.cursorrules` and `tool-specific/cursor-workflow/` context files.

AI assisted across all lifecycle phases — planning, design docs, implementation (Phases 4–6), debugging (CORS/loading issue), integration tests (Phase 7), code review (Phase 9), and documentation (Phase 10). Prompt history in `ai-prompts/`; overall workflow draft in `final-ai-usage-summary.md`.

**Validation:** All AI-generated code reviewed; state machine verified by integration tests + manual UI; persistence verified via API restart test.

---

## Known Limitations

### Intentional (Core / Stretch)

- Demo JWT auth only — no passwords; role claims issued but not enforced on endpoints
- No Blazor route guards — browsing without login; mutations require sign-in
- No pagination, audit log, or file attachments
- No real-time updates

### Documented Technical Debt (Deferred)

- Duplicated state machine rules in `TicketWorkflowService` (UI mirror)
- No unit test tier separate from integration tests
- Production security hardening (rate limiting, CSP, strict CORS) not implemented
- See `review-fixes.md` — 4 fixes applied, 48 findings deferred with rationale

---

## How to Review

### Setup

```bash
git clone <repository-url>
cd ai-practical-assessment

# Database
cd src/TicketSystem.Api
dotnet ef database update

# Terminal 1 — API
dotnet run --launch-profile http

# Terminal 2 — Blazor
cd ../TicketSystem.Blazor
dotnet run --launch-profile http

# Tests
cd ../../tests/TicketSystem.Tests
dotnet test
```

Open http://localhost:5036

### Review Focus

1. **`TicketStateMachine.cs`** — transition rules match spec
2. **`StateMachineTransitionTests.cs`** — mandatory test coverage
3. **`ExceptionHandlingMiddleware.cs`** — error shape for invalid transitions
4. **`TicketService.ChangeStatusAsync`** — state machine integration
5. **Blazor `TicketWorkflowService`** — UI mirrors rules (presentation only)
6. **`code-review-notes.md` / `review-fixes.md`** — conscious scope decisions

### Suggested Test Flow

1. Create ticket → appears in list
2. Open → InProgress → Resolved → Closed (each step succeeds)
3. Attempt Open → Resolved via API → 400 with clear message
4. Add comment → persists on reload
5. Search "database" / filter by status
6. Restart API → ticket still exists

---

## Checklist Before Merge

- [x] All 28 tests passing
- [x] Build succeeds (0 warnings on last verified build)
- [x] README.md with setup instructions
- [x] Database migrations included
- [x] Seed data in migration
- [x] No secrets in repository
- [x] `.gitignore` configured
- [x] Core acceptance criteria met
- [x] Code review completed (`code-review-notes.md`)
- [x] Targeted review fixes applied (`review-fixes.md`)
- [ ] `reflection.md` — candidate to complete
- [ ] `tool-workflow.md` — candidate to complete

---

## Related Documentation

- `README.md` — Setup and run instructions
- `implementation-plan.md` — Phase breakdown
- `design-notes.md` / `api-contract.md` / `data-model.md`
- `test-strategy.md` / `ai-prompts/testing.md`
- `debugging-notes.md` / `ai-prompts/debugging.md`
- `code-review-notes.md` / `review-fixes.md`
- `final-ai-usage-summary.md` — AI workflow across phases
- `ai-prompts/` — Detailed prompt history

---

**Submitted By:** Harshit Gupta  
**Date:** 2026-07-16  
**Estimated Review Time:** 30–45 minutes
