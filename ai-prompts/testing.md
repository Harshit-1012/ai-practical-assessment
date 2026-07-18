# Testing Prompts

## Purpose
Document all AI prompts used during the testing phase (test creation, test data, test strategy).

---


### Prompt #1: State Machine Integration Tests (Phase 7)

**Date:** 2026-07-15
**Context Provided:** implementation-plan.md (Phase 7), completed 
TicketStateMachine implementation

**Prompt:**
Execute Phase 7 from implementation-plan.md: write the mandatory state 
machine integration tests in TicketSystem.Tests, using WebApplicationFactory 
and an in-memory SQLite database.

Required test cases:
- All 5 valid transitions succeed: Open→InProgress, Open→Cancelled, 
  InProgress→Resolved, InProgress→Cancelled, Resolved→Closed
- Invalid transitions are rejected with 400: Open→Resolved, 
  Open→Closed, Resolved→InProgress, Closed→Open, Cancelled→InProgress, 
  and any transition attempted from a terminal state (Closed, Cancelled)

Also add basic integration tests for: ticket CRUD, comment creation, 
and search/filter. Keep test setup clean with reusable test helpers 
(DbContext factory, test data seeder). Stop after this so I can review 
before running.

**AI Response Summary:**
Generated 28 tests across StateMachineTransitionTests (16), 
TicketCrudTests (5), CommentIntegrationTests (3), 
TicketSearchFilterTests (4).

**What Was Accepted:** Test structure and coverage as-is

**What Was Changed:**
- Initial run failed all 28 tests: SQL Server and SQLite providers both 
  registered in the service container, causing a conflict. Asked Cursor 
  to fix the WebApplicationFactory setup to properly remove the SQL 
  Server DbContext registration before adding SQLite for tests.

**What Was Rejected:** None

**Iteration Count:** 2 (initial generation + provider conflict fix)

**Outcome:** All 28 tests passing (28 Passed, 0 Failed, 0 Skipped, 
1.7 sec)


## Manual UI Verification — 2026-07-15

Verified end-to-end through the browser (http://localhost:5036):
- Created a new ticket via the UI — appeared correctly in the list
- Changed status through valid transitions (Open → InProgress → 
  Resolved → Closed) — buttons updated correctly at each stage
- Confirmed invalid transition buttons were disabled/hidden as expected
- Added a comment to a ticket — displayed with author and timestamp
- Tried search by keyword and filter by status — both worked correctly

**Result:** All Core acceptance criteria confirmed working through 
manual UI testing, in addition to the 28 automated integration tests.

---

### Prompt #2: Stretch Unit Test Tier

**Date:** 2026-07-18
**Context Provided:** 28 integration tests in `IntegrationTests/`; Stretch 
scope for a separate unit test tier per spec and review-fixes deferrals

**Prompt:**
```
Add a unit test tier as a Stretch feature, separate from the existing 
integration tests:
- Unit tests for TicketStateMachine.CanTransition() and 
  ValidateTransition() in isolation (no HTTP/database), covering every 
  valid transition and every invalid transition pair
- Unit tests for key validation logic (whitespace rejection, terminal 
  ticket edit blocking) if easily testable in isolation

Keep these clearly separate from IntegrationTests folder — put them in 
a new UnitTests folder within TicketSystem.Tests. Don't modify any 
existing integration tests or application code.

Also log this exact prompt in ai-prompts/testing.md following the same 
template as other entries.

Stop after implementation so I can review before running tests.
```

**AI Response Summary:**
Added `tests/TicketSystem.Tests/UnitTests/` with three test classes (no 
changes to integration tests or application code): `TicketStateMachineTests` 
— 5 valid transitions, all 20 invalid pairs via generated `MemberData`, 
plus `ValidateTransition` throw/no-throw and message assertions; 
`TicketDtoValidationTests` — `[Required]`/`[MaxLength]` on 
`CreateTicketDto`, `UpdateTicketDto`, `CreateCommentDto` via 
`Validator.TryValidateObject`; `TicketEnumValidationTests` — 
`TicketStatusExtensions.IsValid` and `TicketPriorityExtensions.IsValid`. 
Whitespace-only and terminal-ticket edit guards are documented in 
`review-fixes.md` but not present in current application code, so they 
were not unit-tested without modifying `TicketService`.

**What Was Accepted:**
- `UnitTests/` folder separate from `IntegrationTests/`
- Exhaustive state machine pair coverage without HTTP/DB
- DTO and enum validation as isolated unit tests

**What Was Changed:** None at implementation time — stopped for review 
before running tests

**What Was Rejected:** None

**Iteration Count:** 1

**Outcome:** Test project builds successfully. Unit tests not run during 
this pass.
