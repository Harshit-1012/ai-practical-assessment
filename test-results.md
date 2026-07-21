# Test Results

## Test Execution Summary

**Date:** 21-07-2026  
**Environment:** Development / Local  
**Test Framework:** xUnit  
**Database:** SQLite In-Memory (`:memory:` via `CustomWebApplicationFactory`; `Testing` environment)

---

## Overall Results

| Test Category | Total | Passed | Failed | Skipped |
|--------------|-------|--------|--------|---------|
| State Machine Unit Tests | 53 | 53 | 0 | 0 |
| State Machine Integration Tests | 16 | 16 | 0 | 0 |
| Ticket CRUD Tests | 5 | 5 | 0 | 0 |
| Comment Tests | 3 | 3 | 0 | 0 |
| Validation Tests | 29 | 29 | 0 | 0 |
| Search/Filter Tests | 4 | 4 | 0 | 0 |
| **TOTAL** | **110** | **110** | **0** | **0** |

*Breakdown: 82 unit tests (`UnitTests/`) + 28 integration tests (`IntegrationTests/`).*

---

## State Machine Test Results (CRITICAL)

### Unit Tests — `TicketStateMachineTests` (53 cases)

**Valid transition pairs (5 each for `CanTransition` and `ValidateTransition`):**
- [x] `CanTransition_ValidPair_ReturnsTrue` — Open→InProgress, Open→Cancelled, InProgress→Resolved, InProgress→Cancelled, Resolved→Closed — **PASS**
- [x] `ValidateTransition_ValidPair_DoesNotThrow` — same 5 pairs — **PASS**

**Invalid transition pairs (20 each for `CanTransition` and `ValidateTransition`):**
- [x] `CanTransition_InvalidPair_ReturnsFalse` — all 20 invalid status pairs — **PASS**
- [x] `ValidateTransition_InvalidPair_ThrowsInvalidTransitionException` — all 20 invalid pairs — **PASS**

**Additional unit cases:**
- [x] `CanTransition_SameStatus_ReturnsFalse` — **PASS**
- [x] `ValidateTransition_FromClosed_ThrowsWithTerminalMessage` — **PASS**
- [x] `ValidateTransition_FromOpenToResolved_ThrowsWithValidTargetsMessage` — **PASS**

### Integration Tests — `StateMachineTransitionTests` (16 cases)

**Valid transitions (5 theory cases):**
- [x] `ChangeStatus_ValidTransition_Returns200AndPersistsStatus` — Open→InProgress — **PASS**
- [x] `ChangeStatus_ValidTransition_Returns200AndPersistsStatus` — Open→Cancelled — **PASS**
- [x] `ChangeStatus_ValidTransition_Returns200AndPersistsStatus` — InProgress→Resolved — **PASS**
- [x] `ChangeStatus_ValidTransition_Returns200AndPersistsStatus` — InProgress→Cancelled — **PASS**
- [x] `ChangeStatus_ValidTransition_Returns200AndPersistsStatus` — Resolved→Closed — **PASS**

**Invalid transitions (11 theory cases):**
- [x] `ChangeStatus_InvalidTransition_Returns400BadRequest` — Open→Resolved — **PASS**
- [x] `ChangeStatus_InvalidTransition_Returns400BadRequest` — Open→Closed — **PASS**
- [x] `ChangeStatus_InvalidTransition_Returns400BadRequest` — Resolved→InProgress — **PASS**
- [x] `ChangeStatus_InvalidTransition_Returns400BadRequest` — Closed→Open — **PASS**
- [x] `ChangeStatus_InvalidTransition_Returns400BadRequest` — Cancelled→InProgress — **PASS**
- [x] `ChangeStatus_InvalidTransition_Returns400BadRequest` — Closed→InProgress — **PASS**
- [x] `ChangeStatus_InvalidTransition_Returns400BadRequest` — Closed→Resolved — **PASS**
- [x] `ChangeStatus_InvalidTransition_Returns400BadRequest` — Closed→Cancelled — **PASS**
- [x] `ChangeStatus_InvalidTransition_Returns400BadRequest` — Cancelled→Open — **PASS**
- [x] `ChangeStatus_InvalidTransition_Returns400BadRequest` — Cancelled→Resolved — **PASS**
- [x] `ChangeStatus_InvalidTransition_Returns400BadRequest` — Cancelled→Closed — **PASS**

**Notes:**
Valid integration cases assert HTTP 200, response body status, and persisted database status via `GetTicketById`. Invalid cases assert HTTP 400 with structured `ApiError` (transition message, `currentStatus`, `requestedStatus`) and unchanged persisted status. Status changes run under Agent authentication (`AgentIntegrationTestBase`).

---

## CRUD Operation Test Results

### `TicketCrudTests` (5 integration tests)

**Ticket creation:**
- [x] `CreateTicket_ValidData_Returns201Created` — **PASS**

**Ticket retrieval:**
- [x] `GetTicketById_ExistingTicket_Returns200Ok` — **PASS**
- [x] `GetTicketById_NotFound_Returns404` — **PASS**
- [x] `GetTickets_ReturnsAllCreatedTickets` — **PASS** (paged `TicketListResponseDto`)

**Ticket update:**
- [x] `UpdateTicket_ValidData_Returns200Ok` — **PASS** (creates as User role, re-authenticates as Agent before PUT)

**Notes:**
Create/comment tests use default User-role JWT (user id 3). Update test switches to Agent (user id 2) before `PUT /api/tickets/{id}` to satisfy role-based authorization.

---

## Comment Test Results

### `CommentIntegrationTests` (3 integration tests)

- [x] `CreateComment_ValidData_Returns201Created` — **PASS**
- [x] `GetComments_AfterCreation_ReturnsCommentList` — **PASS**
- [x] `CreateComment_TicketNotFound_Returns404` — **PASS**

**Notes:**
Comment creation remains permitted for all authenticated roles (User, Agent, Admin). `CreatedById` derived from JWT claims.

---

## Search and Filter Test Results

### `TicketSearchFilterTests` (4 integration tests)

- [x] `GetTickets_FilterByStatus_ReturnsMatchingTicketsOnly` — **PASS**
- [x] `GetTickets_SearchByKeyword_ReturnsMatchingTickets` — **PASS**
- [x] `GetTickets_FilterByStatusAndKeyword_ReturnsIntersection` — **PASS**
- [x] `GetTickets_InvalidStatusFilter_Returns400BadRequest` — **PASS**

**Notes:**
List endpoint returns `TicketListResponseDto` with pagination metadata. Advanced filtering (priority, assignee, sort) and pagination covered by Stretch implementation; dedicated automated tests exist for status/keyword filters only.

---

## Validation Test Results

### `TicketDtoValidationTests` (10 unit tests)

- [x] `CreateTicketDto_MissingTitle_HasValidationError` — **PASS**
- [x] `CreateTicketDto_MissingDescription_HasValidationError` — **PASS**
- [x] `CreateTicketDto_WhitespaceOnlyTitle_HasValidationError` — **PASS**
- [x] `CreateTicketDto_WhitespaceOnlyDescription_HasValidationError` — **PASS**
- [x] `CreateTicketDto_TitleExceedsMaxLength_HasValidationError` — **PASS**
- [x] `CreateTicketDto_ValidData_HasNoValidationErrors` — **PASS**
- [x] `UpdateTicketDto_MissingTitle_HasValidationError` — **PASS**
- [x] `CreateCommentDto_MissingMessage_HasValidationError` — **PASS**
- [x] `CreateCommentDto_WhitespaceOnlyMessage_HasValidationError` — **PASS**
- [x] `CreateCommentDto_MessageExceedsMaxLength_HasValidationError` — **PASS**

### `TicketEnumValidationTests` (19 unit tests)

- [x] `TicketStatus_IsValid_KnownValues_ReturnsTrue` — 5 cases (Open, InProgress, Resolved, Closed, Cancelled) — **PASS**
- [x] `TicketStatus_IsValid_InvalidValues_ReturnsFalse` — 5 cases — **PASS**
- [x] `TicketPriority_IsValid_KnownValues_ReturnsTrue` — 4 cases — **PASS**
- [x] `TicketPriority_IsValid_InvalidValues_ReturnsFalse` — 5 cases — **PASS**

**Notes:**
Whitespace validation tests cover Fix #2 (`NotWhitespaceAttribute`). Enum tests validate case-sensitive status/priority parsing used by API query filters.

---

## Test Execution Logs

### Command Used
```bash
dotnet test "d:\ai-practical-assessment\tests\TicketSystem.Tests\TicketSystem.Tests.csproj" --logger "console;verbosity=normal"
```

### Console Output
```
Test Run Successful.
Total tests: 110
     Passed: 110
     Failed: 0
    Skipped: 0
 Total time: 7.8002 Seconds
```

All 110 tests passed on first run with no failures or skips. Unit tests completed in ~150 ms; integration tests used in-memory SQLite with per-test database reset and JWT authentication via `AuthTestHelper`.

---

## Failed Tests Analysis

*No tests failed in the final run (110/110 passed). The following issues caused failures during development and were resolved before this execution:*

### Issue A: Missing Fix #2 / Fix #3 in codebase
**Tests affected:** Whitespace validation and terminal-edit scenarios (documented in `review-fixes.md`; integration suite regressed when fixes were absent from `src/`)  
**Root Cause:** `NotWhitespaceAttribute` and terminal-status edit guard were documented as applied but missing from source  
**Fix Applied:** Re-implemented whitespace validation and terminal edit blocking in API + UI  
**Re-test Result:** **PASS** (110/110)

### Issue B: Role-based authorization — 17 tests returned 403 Forbidden
**Tests affected:** `UpdateTicket_ValidData_Returns200Ok` (1) + `StateMachineTransitionTests` (16 theory cases)  
**Root Cause:** `IntegrationTestBase` authenticated as User (id 3); `PUT /api/tickets/{id}` and `PUT /api/tickets/{id}/status` require Admin or Agent role  
**Fix Applied:** Added `AgentIntegrationTestBase` and `AuthTestHelper.AuthenticateAsAgentAsync`; state machine tests inherit Agent base; update test re-authenticates as Agent before PUT  
**Re-test Result:** **PASS** (110/110)

---

## Coverage Report (Optional)

**Line Coverage:** Not measured in this run  
**Branch Coverage:** Not measured in this run  
**Method Coverage:** Not measured in this run

*Coverage tooling was not run for this assessment submission. Integration tests exercise API endpoints end-to-end; unit tests cover state machine, DTO validation, and enum parsing.*

---

## Manual Testing Verification

### UI Manual Tests Performed
- [x] Created ticket through Blazor UI
- [x] Updated ticket through UI (Agent/Admin role)
- [x] Changed ticket status through UI (Agent/Admin role)
- [x] Added comments through UI (all roles)
- [x] Searched, filtered, sorted, and paginated tickets through UI (`/tickets` + `SearchFilter`)
- [x] Login and logout flow (login-required routing; redirect to `/dashboard`; sign-out returns to `/login`)
- [x] Role-based access verified (User role: Edit/status controls hidden, 403 on API; Agent/Admin: full update/status access)
- [x] Verified data persists after API restart (SQL Server LocalDB)

**Notes:**
Manual verification performed via Blazor UI and Swagger during Phases 5–9 and Stretch work (auth, pagination, dashboard, role restrictions). JWT demo login uses user select (no password). Blazor app requires authentication for all pages; API GET endpoints remain callable without JWT for direct API testing.

---

## Test Environment Details

**Operating System:** Microsoft Windows NT 10.0.26200.0 (Windows 11)  
**.NET SDK:** 10.0.300 (target framework: .NET 9.0)  
**Database:** SQLite in-memory for automated tests; SQL Server LocalDB for manual/API runtime testing  
**IDE:** Cursor AI (primary); Visual Studio 2022 (also used for local debugging)

---

## Known Issues

### Issue 1: Fix #2 / Fix #3 documented but missing from source
**Description:** Whitespace-only input validation (`NotWhitespaceAttribute`) and terminal-ticket edit guard were recorded in `review-fixes.md` but absent from `src/` until re-discovered during Stretch work.  
**Impact:** Medium — validation and edit rules not enforced despite documentation  
**Status:** **Fixed**  
**Notes:** Re-implemented and verified via API + full test suite (Prompt #8, 2026-07-17).

### Issue 2: 17 integration tests failed after role-based authorization
**Description:** After adding `[Authorize(Roles = "Admin,Agent")]` on ticket update/status endpoints, tests authenticating as User role (id 3) received 403 Forbidden.  
**Impact:** High — blocked CI/local test runs until test auth updated  
**Status:** **Fixed**  
**Notes:** `AgentIntegrationTestBase` + `AuthenticateAsAgentAsync` for update/status tests; create/comment tests unchanged on User role (Prompt #14 follow-up, 2026-07-18).

---

## Recommendations

- Add dedicated integration tests for role-based 403 responses (User role attempting PUT update/status).
- Add integration tests for advanced list filters (priority, assignee, sort, pagination) if regression coverage is needed beyond manual verification.
- Run code coverage (`coverlet`) before production hardening to identify untested service branches.

---

## Sign-off

**Tests Executed By:** Harshit Gupta  
**Date:** 21-07-2026  
**Overall Status:** All Pass (110/110)

**Core Acceptance Criteria Met:** YES
- State machine integration tests: **PASS** (16/16)
- Data persistence verified: **PASS** (integration + manual restart)
- All CRUD operations working: **PASS** (5/5 CRUD integration tests + comment tests)
