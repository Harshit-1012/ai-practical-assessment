# Implementation Prompts

## Purpose
Document all AI prompts used during implementation (backend, frontend, database, state machine).

---

## Implementation Prompts Log

[Document each implementation prompt]

### Prompt #1: Backend API Implementation (Phase 5)

**Date:** 2026-07-14
**Context Provided:** implementation-plan.md (Phase 5), spec.md

**Prompt:**
Execute Phase 5: DTOs, TicketStateMachine, TicketService, CommentService,
controllers, validation, exception handling.

**AI Response Summary:**
State machine with 5 valid transitions and terminal state enforcement; 
Ticket/Comment/User services; controllers; DTO validation; global 
exception middleware.

**What Was Accepted:**
- State machine logic, controller/service structure

**What Was Changed:**
- Added Swagger/Swashbuckle manually since .NET 9 default template only 
  provides raw OpenAPI JSON, not interactive Swagger UI

**What Was Rejected:**
- None

**Iteration Count:** 2 (initial implementation + Swagger addition)

**Outcome:** Manually verified via Swagger — user list, ticket creation, 
ticket retrieval, comment creation, and validation error handling all 
working correctly.

### Prompt #2: Add Swagger/OpenAPI Support

**Date:** 2026-07-15
**Context Provided:** Working Phase 5 backend (raw OpenAPI JSON only, no UI)

**Prompt:**
Add Swagger/OpenAPI support (Swashbuckle) to TicketSystem.Api and enable it in the dev environment.

**AI Response Summary:**
Added Swashbuckle package and enabled interactive Swagger UI at /swagger, 
since .NET 9's default template only provides raw /openapi/v1.json.

**What Was Accepted:** Added as-is
**What Was Changed:** None
**What Was Rejected:** None
**Iteration Count:** 1
**Outcome:** Swagger UI available for manual API testing

---

### Prompt #3: Frontend Implementation (Phase 6)

**Date:** 2026-07-15
**Context Provided:** implementation-plan.md (Phase 6), completed backend API

**Prompt:**
```
Execute Phase 6 from implementation-plan.md: create the Blazor 
WebAssembly frontend for the ticket system, with a clean, modern, 
professional UI — not a generic Bootstrap default look.

Design direction:
- Use a cohesive color palette (e.g., a primary accent color + neutral 
  grays), not default Bootstrap blue
- Distinct, colored badges for Status (Open/InProgress/Resolved/Closed/
  Cancelled) and Priority (Low/Medium/High/Critical) — each status/
  priority should have its own recognizable color
- Card-based layout for the ticket list (not a plain table) with subtle 
  shadows and hover states
- Clean typography with proper spacing/hierarchy
- A simple top navbar or sidebar with the app name/logo
- Empty states with helpful messaging, loading states with spinner
- Toast/inline notifications for success and error states
- Disabled/grayed-out buttons for invalid status transitions, with a 
  tooltip explaining why

Architecture constraint: Keep all business logic (validation, state 
transition rules, data formatting/decisions) in the Services layer or 
API responses — NOT in the Razor components. Components should only 
handle rendering, input binding, and calling the API service layer.

Build:
- TicketApiService and CommentApiService (HTTP client services)
- TicketList.razor, TicketDetail.razor, CreateTicket.razor, 
  EditTicket.razor
- Reusable components: StatusBadge, PriorityBadge, CommentList, 
  AddComment, SearchFilter
- Remove default Counter/Weather template pages

Stop after this so I can review.
```

**AI Response Summary:**
Built TicketApiService/CommentApiService/UserApiService for HTTP calls; 
TicketWorkflowService for transition rules; TicketDisplayService for 
badge/label/formatting logic; NotificationService for toasts. Pages: 
TicketList, TicketDetail, CreateTicket, EditTicket. Custom theme (Inter 
font, indigo/slate palette), no Bootstrap dependency. Removed default 
Counter/Weather/Home template pages.

**What Was Accepted:** Full architecture and UI design as delivered — 
logic correctly separated into services layer per constraint
**What Was Changed:** [Manual review ke baad yahan likho]
**What Was Rejected:** None
**Iteration Count:** 1
**Outcome:** Frontend builds with 0 warnings, 0 errors — ready for 
manual review


### Prompt #4: Fix Loading Issue + Sidebar Styling

**Date:** 2026-07-15
**Context Provided:** Running Blazor app stuck on "Loading tickets..." 
indefinitely; sidebar nav links appearing as plain underlined text

**Prompt:**
Two things to fix:

The sidebar navigation looks unstyled — style as proper nav items:
no underline, icon + label, hover state, active/selected state,
matching the indigo/slate theme.
The ticket list page is stuck on "Loading tickets..." indefinitely.
Check console/network errors, verify API base URL matches running
port, check CORS configuration. Fix root cause.

**AI Response Summary:**
Sidebar issue: nav styles were in MainLayout.razor.css but links live in 
NavMenu.razor — Blazor CSS isolation doesn't cross component boundaries, 
and NavLink renders its own `<a>` so scoped selectors never applied. 
Fixed by moving styles to NavMenu.razor.css with `::deep`, plus a global 
fallback in app.css.

Loading issue: multiple compounding causes — (1) UseHttpsRedirection() 
ran before UseCors() in Program.cs, causing Blazor's HTTP call to get 
redirected to HTTPS and CORS to fail silently; (2) SearchFilter used 
@bind:after which could re-trigger loads and keep isLoading stuck true; 
(3) no HttpClient timeout, so failures could hang ~100s; (4) API not 
running would produce the same symptom until timeout.

**What Was Accepted:**
- Middleware reordering (CORS before HTTPS redirect, disabled in Dev)
- Explicit allowed origins list
- SearchFilter changed to value + @onchange
- HttpClient 30s timeout added
- NavMenu.razor.css with ::deep + app.css fallback for sidebar

**What Was Changed:** None further — verified fix directly

**What Was Rejected:** None

**Iteration Count:** 1

**Outcome:** Verified both apps build with 0 warnings/errors. Confirmed 
via browser: sidebar shows proper nav styling with hover/active states; 
ticket list loads correctly; CORS preflight from localhost:5036 succeeds 
(204 with correct Access-Control-Allow-Origin header).

---

### Prompt #5: Stretch JWT Authentication (Minimal)

**Date:** 2026-07-16
**Context Provided:** Completed Core backend + Blazor frontend; auth 
deferred as Stretch in spec.md and review-fixes.md

**Prompt:**
```
Implement basic authentication as a Stretch feature, scoped minimally:
- Simple login endpoint using the seeded users (no password — just 
  select a user to "log in as", since there's no real password field 
  in the current User entity) OR add a simple password field if time 
  permits
- JWT token issued on login
- [Authorize] on ticket/comment mutation endpoints only (not read 
  endpoints, to keep it simple)
- CreatedById derived from the authenticated user's claims instead of 
  client-supplied/dropdown value
- Basic login page in Blazor, store token, attach to HTTP requests

Keep this scoped and minimal — do not touch existing working Core 
functionality more than necessary. Stop after implementation so I can 
review before running tests.
```

**AI Response Summary:**
API: `POST /api/auth/login` accepts `{ userId }` (no password — demo 
"sign in as" using seeded users); `JwtTokenService` issues JWT with 
`NameIdentifier`, `Name`, `Email`, `Role` claims; `[Authorize]` on 
ticket/comment mutation endpoints only (POST/PUT tickets, POST comments); 
read endpoints remain anonymous; `CreatedById` removed from 
`CreateTicketDto`/`CreateCommentDto` and set server-side from 
`User.GetUserId()`; JWT config in `appsettings.json`. Blazor: `/login` 
page with user dropdown; `TokenStorageService` (localStorage); 
`CustomAuthStateProvider`; `ApiClientHelper` attaches Bearer token; 
removed "Created By" dropdown from create ticket and hardcoded comment 
author; top bar shows signed-in user + Sign Out. Test helpers added 
(`AuthTestHelper`, updated `TestDataSeeder`) but tests not run per 
request.

**What Was Accepted:**
- Passwordless user-select login approach (no User entity migration)
- JWT on mutations only; reads stay anonymous
- Server-side `CreatedById` from claims
- Blazor token storage + login page

**What Was Changed:** None at implementation time — stopped for review 
before running tests

**What Was Rejected:** Password field on User entity (chose simpler 
user-select login instead)

**Iteration Count:** 1

**Outcome:** Solution builds successfully. Integration tests not run 
during this pass — subsequent testing revealed setup issues (see 
Prompt #6).

---

### Prompt #6: Fix Integration Tests After JWT Auth (24 × 401)

**Date:** 2026-07-17
**Context Provided:** JWT auth implemented (Prompt #5); 24 of 28 
integration tests failing with 401 Unauthorized

**Prompt:**
```
I added JWT authentication, and now 24 integration tests are failing with 401 Unauthorized.

It looks like the tests are not authenticating before making API requests.

Please check whether AuthTestHelper is actually being called in the integration tests. If it isn't, fix it so that every test requiring authentication logs in first and attaches the Bearer token to the HttpClient.

Please review these test classes:

StateMachineTransitionTests
TicketCrudTests
CommentIntegrationTests
TicketSearchFilterTests

Also explain where the authentication flow is breaking and what changes you made.

After fixing authentication, investigate the remaining failures:

GetTickets_ReturnsAllCreatedTickets (expected 2, got 0)
GetTickets_SearchByKeyword

These may indicate that the test data isn't being created correctly or the test setup has broken. Fix the root cause instead of adding workarounds.

Finally, rerun the tests and make sure all integration tests pass.
```

**AI Response Summary:**
`AuthTestHelper` was correctly wired in `IntegrationTestBase.InitializeAsync()`, 
but **xUnit does not call `IAsyncLifetime.InitializeAsync()` on abstract 
base classes** — only when the concrete test class explicitly implements 
the interface. So reset + login never ran; `HttpClient` had no Bearer 
token; all mutation endpoints returned 401. The four passing tests were 
read-only or seeded data directly via EF (`SeedTicketAsync`), so they 
never needed auth. Fixed by moving `ResetDatabaseAsync()` + 
`AuthenticateAsUserAsync()` into the `IntegrationTestBase` constructor 
(guaranteed to run before every test). Also aligned test JWT secret with 
`appsettings.json` and added `EnsureUsersSeededAsync()` in 
`CustomWebApplicationFactory` for SQLite in-memory. 
`GetTickets_ReturnsAllCreatedTickets` and `GetTickets_SearchByKeyword` 
failures were downstream of 401 on `POST /api/tickets` (no tickets 
created to search), not separate bugs.

**What Was Accepted:**
- Constructor-based reset + auth instead of `IAsyncLifetime` on base class
- JWT secret alignment in test factory
- Explicit user seeding in test factory

**What Was Changed:** None further after fix

**What Was Rejected:** Workarounds for search/list tests (root cause was 
missing auth, not broken search logic)

**Iteration Count:** 1

**Outcome:** All 28 integration tests passing.

---

### Prompt #7: Stretch Advanced Filtering and Sorting

**Date:** 2026-07-17
**Context Provided:** Core search/status filter working; Stretch scope for 
pagination/advanced filtering per spec

**Prompt:**
```
Implement advanced filtering and sorting as a Stretch feature:
- Filter tickets by priority (in addition to existing status filter)
- Filter tickets by assignee
- Sort by CreatedAt, Priority, or Status (ascending/descending)
- Update the API's GET /api/tickets endpoint to accept these as query 
  parameters
- Update the Blazor SearchFilter component and TicketList page to 
  expose these controls

Keep this scoped — don't touch authentication, state machine, or 
existing working search/status-filter behavior.

Also add a new entry to ai-prompts/implementation.md logging this exact 
prompt, following the same template as the other entries (Date, Context 
Provided, Prompt, AI Response Summary, Accepted/Changed/Rejected, 
Iteration Count, Outcome) — fill in the AI Response Summary and Outcome 
sections based on what you actually build.

Stop after implementation so I can review before running tests.
```

**AI Response Summary:**
API: Extended `GET /api/tickets` with query parameters `priority`, 
`assignedToId`, `unassignedOnly`, `sortBy`, `sortDirection` (existing 
`status` and `keyword` unchanged). Added `TicketListQueryDto`; 
`TicketService` validates priority/assignee/sort inputs, applies filters 
in EF query, sorts in-memory by logical priority/status order or 
`CreatedAt` (default `CreatedAt` desc preserved when sort params omitted). 
Blazor: expanded `TicketSearchCriteria` and `TicketApiService` query 
string builder; `SearchFilter` now has priority, assignee (All/Unassigned/
users), sort-by, and order dropdowns with auto-apply on change; 
`TicketList` tracks and passes initial filter state; `TicketDisplayService` 
gained filter/sort option helpers; CSS updated for multi-row filter layout.

**What Was Accepted:**
- Query-parameter approach on existing endpoint (no new routes)
- In-memory sort for Priority/Status logical ordering
- Assignee filter via `assignedToId` + `unassignedOnly` flags
- Auto-apply on dropdown change (matching existing status filter behavior)

**What Was Changed:** None at implementation time — stopped for review 
before running tests

**What Was Rejected:** None

**Iteration Count:** 1

**Outcome:** Solution builds successfully with 0 warnings/errors. 
Integration tests not run during this pass.
