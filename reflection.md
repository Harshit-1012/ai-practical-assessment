# Reflection

## What I Built

### Project Summary
I built a Support Ticket Management System — the Backend-Heavy Core option from the .NET AI Capability Exercise. Internal users can create tickets, view and update them, move them through a fixed status lifecycle, add comments, and search/filter the ticket list. The signature piece is a backend-enforced state machine that only allows specific status transitions (Open → InProgress → Resolved → Closed, plus Cancelled paths) and rejects everything else with a clear error, both at the API level and in the UI (invalid actions are disabled rather than just failing silently).

### Technical Stack
- **Frontend:** Blazor WebAssembly
- **Backend:** ASP.NET Core Web API (.NET 9)
- **Database:** SQL Server (LocalDB) with Entity Framework Core
- **Testing:** xUnit with WebApplicationFactory + in-memory SQLite

### Core Features Delivered
1. Ticket CRUD (create, list, detail, update)
2. Status state machine with backend-enforced valid/invalid transitions
3. Comments on tickets
4. Keyword search and status filtering
5. Data persistence verified across an actual API restart
6. Server-side validation and structured error responses
7. 28 passing integration tests covering the state machine, CRUD, comments, and search/filter
8. AI-assisted code review with 4 fixes applied and 48 findings consciously deferred with documented rationale

### Stretch Features


---

## How I Used AI

### Primary Tool
**Cursor AI** — mainly Claude Sonnet 4.5, with Cursor's Auto mode used for some simpler/routine tasks.

### Context Provision Strategy

**Approach:**
Before writing any code, I set up persistent context: a `.cursorrules` file with .NET 9/Blazor/EF Core conventions and the state machine rules, plus `project-context.md`, `spec.md`, and `tasks.md` under `tool-specific/cursor-workflow/`. From there, I worked phase-by-phase off an `implementation-plan.md`, referencing it directly in prompts (e.g., "Execute Phase 5 from implementation-plan.md...") rather than re-explaining requirements each time.

**What Worked Well:**
- Referencing the plan file and spec directly in prompts kept output aligned with actual requirements instead of generic scaffolding.
- Stating architecture constraints upfront (e.g., "keep business logic in Services, not Razor components") avoided rework later.
- Giving Cursor the symptom plus an instruction to investigate ("check console/network errors, verify CORS") rather than just "fix it" led to much better root-cause diagnosis.

**What Didn't Work:**
- Early on, I let a couple of prompts run too broad ("execute this whole phase") without enough intermediate checkpoints, which made it harder to review incrementally and cost more in back-and-forth than doing it in smaller chunks.
- I initially treated "Keep All" / file-apply confirmations as optional and skipped one, which briefly caused confusion about whether changes were actually saved to disk.

### Typical Workflow

**My Pattern:**
1. Reference the relevant phase from `implementation-plan.md` plus any specific architecture/design constraints.
2. Let Cursor generate the implementation, then stop and review before continuing.
3. Manually test (Swagger, browser UI, or `dotnet test`) before accepting.
4. Log the prompt and outcome in the matching `ai-prompts/*.md` file.

**Example Interaction:**
```
Me: "Execute Phase 5: create DTOs, implement TicketStateMachine with the 
5 valid transitions, TicketService, CommentService, and controllers with 
input validation and global exception handling."
AI: [Generated the full backend layer, state machine, and controllers]
Me: [Manually tested via Swagger — created a ticket, walked it through 
Open → InProgress → Resolved → Closed, then tried an invalid Open → 
Closed jump to confirm it was rejected]
```

---

## What AI Helped With Most

### 1. State Machine Implementation
**Activity:** TicketStateMachine design and implementation
**How AI Helped:** Generated the transition-validation logic matching the spec's exact rules on the first pass, including correctly treating Closed and Cancelled as terminal states.
**Time Saved:** Would have taken meaningfully longer to hand-write and manually verify every transition combination.
**Quality Impact:** The logic held up under both automated (16 transition tests) and manual testing without needing a rewrite.

### 2. Integration Test Generation
**Activity:** Writing the 28 integration tests (state machine, CRUD, comments, search/filter)
**How AI Helped:** Produced a full WebApplicationFactory-based test setup with in-memory SQLite, covering all 5 valid transitions and the invalid/terminal-state cases in one pass.
**Time Saved:** Significant — hand-writing this scaffolding (test factory, seeders, theory data for every transition pair) would have taken much longer than reviewing and fixing the one provider-conflict issue that came up.
**Quality Impact:** Gave me concrete, repeatable proof the state machine works, not just manual spot-checks.

### 3. Code Review
**Activity:** Phase 9 AI-assisted review of backend and frontend
**How AI Helped:** Surfaced specific, file-level issues I hadn't noticed — a hardcoded comment author, whitespace-only input slipping past `.Trim()`, terminal tickets still being editable, and a Blazor routing bug where navigating between ticket IDs didn't reload data.
**Time Saved:** Manually reviewing every file for these kinds of issues would have taken much longer than triaging a generated list.
**Quality Impact:** All 4 issues were real and fixed; the review also gave me a structured way to consciously scope out 48 lower-priority items rather than ignoring them silently.

### Areas Where AI Excelled
- Boilerplate generation (entities, DTOs, controllers, Blazor pages)
- Test scaffolding for a non-trivial business rule (the state machine)
- Broad, severity-rated code review across backend, frontend, and tests in one pass
- Drafting documentation directly from the actual implemented code rather than generic templates

---

## What AI Got Wrong

### Mistake #1: Database Provider Conflict in Tests
**What AI Suggested:**
The initial test project registered both the SQL Server provider (from Program.cs) and an in-memory SQLite provider for tests, without properly removing the SQL Server registration first.

**Why It Was Wrong:**
EF Core only allows a single database provider to be registered per service container — having both caused every single test (all 28) to fail immediately with an `InvalidOperationException`, unrelated to the actual test logic.

**How I Caught It:**
Ran `dotnet test` and read the error message closely — it clearly named the provider conflict rather than a logic failure, which pointed me toward asking Cursor to fix the WebApplicationFactory setup specifically.

**Correct Solution:**
Cursor added an explicit environment gate that removes the SQL Server DbContext registration before adding the SQLite one for the `Testing` environment.

**Lesson:**
Test infrastructure failures can look alarming (all tests red) but are often configuration issues, not business-logic bugs — worth reading the actual exception type before assuming the worst.

### Mistake #2: CORS/Middleware Ordering Causing Infinite Loading
**What AI Suggested:**
The initial Program.cs registered `UseHttpsRedirection()` before `UseCors()`.

**Why It Was Wrong:**
This caused the Blazor app's HTTP call to the API to get silently redirected to HTTPS, which broke CORS and left the ticket list stuck on "Loading tickets..." indefinitely, with no visible error.

**How I Caught It:**
Noticed the infinite loading spinner during manual UI testing and asked Cursor to investigate console/network errors and CORS configuration specifically, rather than guessing at UI-side fixes.

**Correct Solution:**
Reordered the middleware (CORS before HTTPS redirect), disabled HTTPS redirect in Development, added explicit allowed origins, and added an HttpClient timeout so future failures would surface faster instead of hanging.

**Lesson:**
A stuck loading state can have several compounding causes at once (middleware order, binding pattern, missing timeout) — worth asking for a full root-cause investigation rather than accepting the first plausible fix.

### Mistake #3: Misjudging Priority in Code Review
**What AI Suggested:**
During the Phase 9 review, AI ranked "no authentication/authorization" as a top-priority finding to fix.

**Why It Was Wrong:**
The spec explicitly lists authentication as a Stretch feature, not a Core requirement — treating it as top priority would have pulled focus away from Core lifecycle artifacts.

**How I Caught It:**
Cross-checked the finding against `spec.md`'s Core vs. Stretch boundaries before accepting the review's prioritization.

**Correct Solution:**
Deferred authentication (and the related client-supplied `CreatedById` trust issue) to `review-fixes.md` with explicit reasoning tied to the spec, instead of implementing it.

**Lesson:**
AI code review is good at finding issues but doesn't automatically know which ones matter for a given scope — that judgment call has to stay with me.

### Patterns in AI Mistakes
The recurring theme wasn't AI writing broken logic — the state machine and core business rules were correct from early on. The mistakes were mostly in **configuration/infrastructure details** (service registration order, middleware order) and in **scope judgment** (treating every review finding as equally urgent regardless of what the spec actually required).

---

## How I Validated AI Output

### Validation Checklist I Used
- [x] Read and understood all generated code
- [x] Tested code functionality
- [x] Ran automated tests
- [x] Verified against requirements
- [x] Reviewed for best practices (via code review pass)
- [x] Manual testing in UI
- [x] Database verification (persistence across restart)

### Specific Validation Examples

**Example 1: State Machine Logic**
- **AI Output:** TicketStateMachine class with transition validation
- **My Validation:**
  1. Manually walked a ticket through the full valid chain (Open → InProgress → Resolved → Closed) via Swagger
  2. Manually attempted invalid jumps (e.g., Open → Closed directly) and confirmed rejection with a 400
  3. Ran the 16 automated transition tests to confirm coverage matched the spec's exact rules
- **Result:** No issues found — logic was correct on first pass.

**Example 2: Persistence Verification**
- **AI Output:** EF Core migration and seed data
- **My Validation:** Created a ticket via the API, stopped the process entirely, restarted it, and confirmed the ticket was still retrievable with identical data — proving storage was actually in SQL Server, not in-memory.
- **Result:** Passed; documented the full procedure in `database/setup-notes.md`.

### Red Flags I Learned to Watch For
1. All tests failing at once (usually infrastructure/config, not logic)
2. UI stuck in a loading/pending state with no visible error (usually a silent network/CORS failure)
3. Code review findings that sound urgent but need to be checked against actual scope before acting on them

---

## What I Would Improve Next Time

### Process Improvements

**What I'd Do Differently:**
1. Log prompts immediately after every Cursor interaction, not sometimes in batches afterward
   - **Why:** Exact prompt wording is easy to lose or paraphrase inaccurately once time passes.
   - **Expected Benefit:** More accurate, verbatim prompt history without needing to reconstruct it later.
2. Commit to git after every phase, not several phases at once
   - **Why:** Smaller commits make the history easier to point to for specific decisions.
   - **Expected Benefit:** Cleaner, more granular commit history that better demonstrates iterative progress.

**What I'd Do the Same:**
1. Setting up `.cursorrules` and context files before writing any code
2. Stopping for manual review after each phase rather than letting everything run unattended

### AI Usage Improvements

**Better Prompting:**
- Break large phase prompts into smaller, more reviewable chunks rather than one very large instruction per phase.

**Better Validation:**
- Test immediately after each sub-feature rather than waiting until an entire phase is "complete" to test everything at once.

**Better Documentation:**
- Keep a running scratch log of prompts as I go, rather than reconstructing summaries afterward.

### Technical Improvements

**Code Quality:**
- Reduce the duplicated state-machine/validation rules between the API and the Blazor `TicketWorkflowService` (currently a UI-only mirror) — this was flagged in code review and deferred due to time.

**Testing:**
- Add a dedicated unit test tier separate from integration tests, and a more exhaustive invalid-transition matrix, as noted in `review-fixes.md`.

**Architecture:**
- If authentication were added later, derive `CreatedById` from an authenticated session instead of a client-supplied dropdown value.

---

## Reusable Workflow

### Assets I Created for Reuse

**1. .cursorrules File**
- Contains: .NET 9/C# conventions, Blazor patterns, EF Core practices, state machine rules
- Reusable for: Future .NET projects
- Key patterns: Terminal-state handling, async/await conventions, structured error responses

**2. Project Context Template**
- Structure under `tool-specific/cursor-workflow/` for giving AI a project overview before implementation begins
- Reusable: Any project using Cursor or similar AI tools

**3. Prompt Templates**
Location: `ai-prompts/` folder
- Phase-scoped execution prompts with explicit stop points
- Constraint-first prompts (stating architecture rules upfront)
- Fix prompts that ask for root-cause investigation, not just "make it work"

**4. Test Strategy Pattern**
- WebApplicationFactory + in-memory SQLite integration test setup
- Transition-matrix approach for testing state machines specifically

**5. Documentation Templates**
All lifecycle documents created (requirements-analysis, design-notes, api-contract, etc.) are directly reusable structures for future projects.

### Workflow I Would Share With Team

1. **Setup Phase:** Create `.cursorrules` and context files before any code; list requirements explicitly.
2. **Implementation Phase:** Use phase-scoped prompts with clear stop points; test immediately; log prompts as you go.
3. **Validation Phase:** Read every line of generated code; test both automated and manually; check logic against requirements, not just "does it run."
4. **Documentation Phase:** Let AI draft from actual code, but personalize anything reflective or judgment-based.
5. **Review Phase:** Use AI review for breadth, but triage every finding against actual project scope before acting.

---

## Key Insights

### About AI Capabilities
1. AI is very strong at producing correct, spec-aligned logic for well-defined business rules (the state machine) when given clear requirements upfront.
2. AI's blind spots are less about "wrong code" and more about configuration subtleties (service registration order, middleware order) and scope judgment (not knowing what's Core vs. Stretch for a specific project).
3. A broad AI code review is genuinely useful for surfacing real issues quickly, but the prioritization it suggests still needs to be checked against the actual spec.

### About My Development Process
1. I under-estimated, initially, how much value there is in stopping after every phase to manually verify before moving on — it caught real issues (loading bug, test conflict) before they compounded.
2. Documenting prompts as I went (rather than after the fact) was more reliable, though I wasn't always disciplined about doing it immediately.

### About the Technology Stack
1. Blazor's CSS isolation doesn't cross component boundaries automatically — styles for a child component (like `NavLink`) need to live in that component's own `.razor.css` file with `::deep`, not the parent layout's.
2. EF Core's service container only tolerates a single registered database provider at a time — mixing SQL Server and SQLite (even across different environments) needs an explicit swap, not just an additional registration.

---

## Honest Assessment

### What Went Well
- The state machine — the assessment's signature piece — worked correctly from an early stage and held up under both automated and manual testing.
- The code review and deferral process gave a clear, honest account of what was and wasn't addressed, rather than pretending everything was fixed.
- Data persistence was explicitly verified with a real restart test, not just assumed.

### What Was Challenging
- Diagnosing the CORS/middleware ordering issue took some iteration before landing on the actual root cause.
- Keeping prompt logging consistently up to date in real time, rather than reconstructing it afterward, required discipline.

### Time Management
**Planned Time:** One week
**Time Breakdown (approximate):**
- Setup and planning: ~2-3 hours
- Implementation (backend + frontend + database): ~6-8 hours
- Testing and debugging: ~2 hours
- Code review and fixes: ~1-2 hours
- Documentation: ~3-4 hours

**If I Had More Time:**


---

## Growth Areas

### Technical Skills to Develop
1. Blazor WebAssembly patterns beyond basic CRUD (state management, component lifecycle nuances)
2. Deeper EF Core testing patterns (provider swapping, test isolation)

### AI Usage Skills to Develop
1. More disciplined, immediate prompt logging
2. Better initial prompt scoping (smaller, more reviewable chunks per prompt)

### Process Skills to Develop
1. More frequent, smaller git commits tied to specific decisions
2. Earlier, incremental testing rather than testing in large batches at the end of a phase

---

## Final Thoughts

This exercise showed me that AI is most valuable as a fast, capable implementer of well-specified logic — but the judgment about what to build, what to defer, and what's actually correct still has to come from me. The state machine and test suite are the clearest evidence of that combination working well; the deferred code-review items are evidence of conscious scope decisions rather than blind AI trust. Going forward, I'd keep the same core discipline — persistent context, phase-by-phase review, and honest documentation of what AI got wrong — on any AI-assisted project.

---

**Reflection Completed By:** Harshit Gupta
**Date:** 2026-07-16
**Overall Experience:** Positive
**Would Use This Approach Again:** Yes, with the same guardrails (persistent context, phased review, mandatory testing)