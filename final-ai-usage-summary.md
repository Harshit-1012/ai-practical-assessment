# Final AI Usage Summary

## Executive Summary

**Project:** Support Ticket Management System  
**Duration:** 2026-07-14 to 2026-07-16  
**Primary AI Tool:** Cursor AI (Agent mode)  
**Overall AI Impact:** High

AI was used across the full software lifecycle — from repository scaffolding and planning artifacts through backend/frontend implementation, debugging, integration testing, code review, and documentation. The workflow was anchored by persistent context (`.cursorrules`, `tool-specific/cursor-workflow/`, phase-scoped prompts in `ai-prompts/`) and a consistent pattern of **generate → review → test → iterate**.

**Candidate-authored separately:** `reflection.md` (personal learnings) and `tool-workflow.md` (Part A workflow foundation) — not generated in this document.

---

## AI Tool Configuration

### Tool Details

| Setting | Value |
|---------|-------|
| Tool | Cursor AI |
| IDE | Cursor |
| Context files | `.cursorrules`, `spec.md`, `implementation-plan.md`, `tool-specific/cursor-workflow/*` |
| Approach | Phase-based prompts with "stop for review" checkpoints |

### Context Strategy

1. **Before coding:** Created `.cursorrules` with .NET 9, Blazor, EF Core, and state machine conventions.
2. **Per phase:** Referenced `implementation-plan.md` and `spec.md` in prompts (e.g., "Execute Phase 5…").
3. **During implementation:** Pointed AI at specific files ("fix `CustomWebApplicationFactory`", "style `NavMenu`").
4. **After implementation:** Asked AI to document *actual* code (`data-model.md`, `api-contract.md`, `design-notes.md`) rather than templates.
5. **Boundaries:** No secrets in prompts; connection strings kept in appsettings / user secrets pattern.

---

## AI Usage by Lifecycle Phase

### Phase 1–2: Foundation & Cursor Context

**Prompts:** See `ai-prompts/planning.md` (Prompts #1–2)

**Activities:**
- Scaffolded repo structure, three .NET projects, documentation templates
- Created `.cursorrules`, `tool-specific/cursor-workflow/` files, `NuGet.config`
- Initialized git repository

**AI contribution:** High — majority of boilerplate structure  
**Validation:** Verified solution builds; corrected `spec.md` location when placed at wrong path  
**Iteration:** 2 rounds (scaffold + path fix)

---

### Phase 3: Requirement Analysis & Planning Artifacts

**Prompts:** See `ai-prompts/planning.md` (Prompts #2–3)

**Activities:**
- Generated `implementation-plan.md` from spec and plan inputs
- Created `requirements-analysis.md` template content from spec
- Produced planning artifact templates (acceptance criteria, test strategy, UI flow, etc.)

**AI contribution:** High for structure and initial content  
**Human edits:** Corrected evaluation weighting in `implementation-plan.md` (Part B = 60%, not documentation alone); filled "Understanding/Assumptions" sections personally where required

---

### Phase 4: Database Design & Implementation

**Prompts:** See `ai-prompts/implementation.md` (database portions)

**Activities:**
- Entity models (`User`, `Ticket`, `Comment`)
- `AppDbContext` Fluent API configurations
- Initial migration `20260714093803_InitialCreate`
- Five-user seed data via `HasData`
- Persistence verification documented in `database/setup-notes.md`

**AI contribution:** High  
**Validation:** `dotnet ef database update`; created ticket via API, restarted API, confirmed ticket persisted  
**Key success:** FK relationships and seed data applied correctly on first migration

---

### Phase 5: Backend API

**Prompts:** See `ai-prompts/implementation.md` (Prompts #1–2)

**Activities:**
- DTOs, services, controllers
- `TicketStateMachine` (signature piece)
- `ExceptionHandlingMiddleware`
- Data Annotations validation
- CORS configuration
- Swashbuckle Swagger UI (added after noting .NET 9 template lacks interactive Swagger)

**AI contribution:** High (~80% initial code)  
**Human changes:** Swagger package addition; manual Swagger verification  
**Validation:** Swagger UI — create ticket, list users, change status, validation errors

---

### Phase 6: Blazor Frontend

**Prompts:** See `ai-prompts/implementation.md` (Prompts #3–4)

**Activities:**
- Client service layer (`TicketApiService`, `TicketWorkflowService`, etc.)
- Pages and reusable components
- Custom indigo/slate theme (explicitly *not* default Bootstrap)
- Removed template pages (Counter, Weather, Home)
- **Debug pass:** Sidebar styling + infinite loading fix (CORS/middleware order, SearchFilter binding, HttpClient timeout)

**AI contribution:** High  
**Human validation:** Browser testing; confirmed CORS preflight and ticket list load  
**Iteration:** 2 (initial build + connectivity/styling fix)

**AI limitation encountered:** Initial nav CSS in `MainLayout.razor.css` did not apply to `NavLink` in child component — required understanding Blazor CSS isolation (`::deep`) and middleware ordering.

---

### Phase 7: Testing

**Prompts:** See `ai-prompts/testing.md` (Prompts #1–2)

**Activities:**
- `WebApplicationFactory` + SQLite in-memory setup
- `TestDataSeeder`, `HttpClientJsonExtensions`, `IntegrationTestBase`
- 28 integration tests (state machine, CRUD, comments, search/filter)
- Fixed dual DbContext provider conflict (`Testing` environment gate in `Program.cs`)

**AI contribution:** High  
**Human validation:** Ran `dotnet test` — 28/28 pass after provider fix  
**Iteration:** 2 (initial tests all failed → factory/environment fix)

---

### Phase 8: Testing & Debugging

**Prompts:** See `ai-prompts/debugging.md`

**Activities:**
- Documented loading-spinner / CORS issue
- Documented test provider conflict
- Manual UI verification checklist (create, transitions, comments, search/filter)

**AI contribution:** Medium — diagnosis and fix suggestions; human confirmed in browser and test runner  
**Outcome:** Issues resolved and recorded in `ai-prompts/debugging.md`

---

### Phase 9: Code Review & Refinement

**Prompts:** See `ai-prompts/code-review.md`

**Activities:**
- AI-assisted review of backend, frontend, and tests (52 findings)
- Applied **4 targeted fixes** (comment author, whitespace validation, terminal ticket edit guard, route reload)
- Deferred 48 items with documented rationale (`review-fixes.md`)

**AI contribution:** High for breadth of review  
**Human judgment:** Triaged findings against `spec.md` Core vs Stretch — deferred authentication, security hardening, and extra test tiers  
**Fixes applied:** Documented in `review-fixes.md` with file lists and verification steps

---

### Phase 10: Documentation & Finalization

**Prompts:** See `ai-prompts/documentation.md`

**Activities:**
- Filled `data-model.md`, `api-contract.md`, `design-notes.md` from actual code
- Created `README.md`, `pr-description.md`, this summary
- Left `reflection.md` and `tool-workflow.md` for candidate authorship

**AI contribution:** High for drafting; content cross-checked against source files

---

## Prompt Strategy Patterns

### What Worked Well

**1. Phase-scoped execution prompts**
```
Execute Phase 7 from implementation-plan.md: write mandatory state machine 
integration tests… Stop after this so I can review before running.
```
Clear scope, explicit stop point, references plan file.

**2. Constrained implementation prompts**
```
Architecture constraint: Keep all business logic in Services layer — NOT in 
Razor components. UI: custom theme, not generic Bootstrap.
```
Reduced rework by stating non-negotiables upfront.

**3. Fix prompts with symptoms + investigation ask**
```
Ticket list stuck on "Loading tickets..." — check console/network, API URL, 
CORS. Fix root cause.
```
Better than "make it work" — AI traced middleware order and binding issues.

**4. Document-from-code prompts**
```
Fill data-model.md based on actual implemented code.
```
Avoids docs drifting from reality.

### What to Avoid

- **Over-broad:** "Build the entire ticket system" — too much at once, generic output
- **Assuming context:** "Add validation" without file/method — wrong target
- **Treating all review findings as mandatory** — 52 findings needed triage against scope

---

## Validation Approach

### Standard checks for AI-generated code

1. Read and understand generated code
2. `dotnet build` — 0 warnings target
3. `dotnet test` — 28 integration tests
4. Manual UI smoke test for user-facing changes
5. Swagger for API contract verification
6. Compare docs to actual source files

### Extra rigor for state machine

1. Read `TicketStateMachine.cs` against spec transition table
2. Run `StateMachineTransitionTests` (16 cases)
3. Manual UI — disabled buttons for invalid transitions
4. API — invalid transition returns structured 400

### When AI was wrong or incomplete

| Issue | AI mistake | Human correction |
|-------|-----------|------------------|
| Sidebar styling | CSS in wrong scoped file | `NavMenu.razor.css` + `::deep` |
| Loading hang | Did not initially identify middleware order | CORS before HTTPS redirect |
| Test factory | Both SQL providers registered | `Testing` env gate in `Program.cs` |
| Code review | Auth flagged as top fix | Deferred — Stretch per spec |
| Evaluation % | Wrong weight in plan | Manual correction in plan doc |

---

## AI Impact Metrics (Estimated)

| Metric | Estimate |
|--------|----------|
| Prompts across lifecycle | ~15 major phase prompts + ~5 fix/review prompts |
| Code initially AI-generated | ~75–85% |
| Accepted with minor/no edits | ~60% (entities, services, tests, docs structure) |
| Required iteration (1+ fix rounds) | ~25% (frontend debug, test factory, Swagger) |
| Rejected or heavily reworked | ~10% (wrong paths, over-scoped suggestions) |
| Integration tests from AI | 28 (all passing after factory fix) |
| Time saved (estimate) | ~15–20 hours vs solo implementation |
| Manual time still required | Review, triage, browser testing, personal reflection docs |

*Exact line counts not measured — estimates based on phase completion patterns.*

---

## What AI Did Exceptionally Well

1. **Boilerplate at speed** — entities, DTOs, controllers, Blazor pages, test scaffolding
2. **State machine implementation** — correct transition table on first pass
3. **Integration test structure** — `WebApplicationFactory`, theory data for transitions
4. **Documentation drafting** — API contract, data model, README from real code
5. **Broad code review** — surfaced validation gaps, routing bugs, security posture quickly
6. **Debugging compound issues** — CORS + binding + timeout identified together

---

## What AI Struggled With

1. **Blazor CSS isolation** — did not initially place nav styles correctly for `NavLink`
2. **Test host configuration** — minimal hosting + `ConfigureTestServices` ordering subtlety
3. **Scope judgment** — review treated auth as must-fix; needed human triage against spec
4. **Production concerns** — suggested many items appropriate for prod but out of assessment scope
5. **Verification** — cannot replace browser/test runner confirmation

---

## Reusable Assets Created

| Asset | Path | Reusable? |
|-------|------|-----------|
| Cursor rules | `.cursorrules` | Yes — adapt per .NET project |
| Workflow context | `tool-specific/cursor-workflow/` | Yes — template for future exercises |
| Prompt logs | `ai-prompts/*.md` | Yes — lifecycle pattern |
| Integration test pattern | `CustomWebApplicationFactory` + `Testing` env | Yes — ASP.NET Core + EF |
| State machine test matrix | `StateMachineTransitionTests` | Yes — transition table driven |
| Doc templates | Root `*.md` files | Yes — lifecycle artifact set |

---

## Recommendations

### For future AI-assisted projects

1. Set `.cursorrules` and context files **before** first code prompt
2. Use **phase prompts with stop points** — review before next phase
3. Always **run tests and manual checks** after AI changes
4. **Triage review findings** against spec scope before implementing
5. Keep **prompt history** as you go — reconstructing later is harder

### For this submission

- Complete `tool-workflow.md` (Part A) and `reflection.md` (Part C) personally
- Optional: fill `test-results.md` with latest `dotnet test` output
- Optional: add screenshots to `pr-description.md` before final submission

---

## Final Assessment

**Overall AI impact rating:** 8/10

AI materially accelerated implementation and documentation while requiring consistent human validation, scope discipline, and iteration on integration/debugging issues. The state machine and test suite are the strongest evidence that AI + verification produces reliable Core functionality.

**Would I use AI again?** Yes — with the same guardrails: persistent context, phased prompts, mandatory tests, and explicit scope boundaries.

**Key takeaway:** AI is most effective as a **fast draft engine** paired with human **scope judgment, testing, and triage** — not as an autonomous implementer.

---

## Appendices

### Appendix A: Prompt log locations

| Phase | File |
|-------|------|
| Planning / setup | `ai-prompts/planning.md` |
| Design | `ai-prompts/design.md` |
| Implementation | `ai-prompts/implementation.md` |
| Testing | `ai-prompts/testing.md` |
| Debugging | `ai-prompts/debugging.md` |
| Code review | `ai-prompts/code-review.md` |
| Documentation | `ai-prompts/documentation.md` |

### Appendix B: High-AI-contribution files

- `src/TicketSystem.Api/Services/*`, `Controllers/*`, `Data/AppDbContext.cs`
- `src/TicketSystem.Blazor/Pages/**`, `Components/**`, `Services/**`
- `tests/TicketSystem.Tests/IntegrationTests/**`
- Lifecycle docs: `api-contract.md`, `data-model.md`, `design-notes.md`, `README.md`

### Appendix C: Primarily human-owned artifacts

- `reflection.md` *(to be completed by candidate)*
- `tool-workflow.md` *(to be completed by candidate)*
- `candidate-info.md` personal details
- Review triage decisions in `review-fixes.md`
- Evaluation weight correction in `implementation-plan.md`

---

**Summary drafted with:** Cursor AI assistance  
**Date:** 2026-07-16  
**Project status:** Core complete — submission artifacts in progress
