# Code Review Prompts

## Purpose
Document all AI prompts used during code review and quality assessment.

---

## Prompt Template

### Prompt #[Number]: [Brief Description]

**Date:** [Date]  
**Review Scope:** [File/Module/Entire project]  
**Review Focus:** [Security/Performance/Best Practices/All]  
**Context Provided:** [Code shared with AI]

**Prompt:**
```
[Exact or summarized prompt text]
```

**AI Response Summary:**
[AI's findings and suggestions]

**Findings Accepted:**
- [Issues that were real and addressed]

**Findings Rejected:**
- [Suggestions not implemented and why]

**Actions Taken:**
[Changes made based on review]

**Iteration Count:** [Number]

---

## Code Review Prompts Log

### Prompt #1: Phase 9 AI-Assisted Code Review

**Date:** 2026-07-15  
**Review Scope:** Entire project — `TicketSystem.Api`, `TicketSystem.Blazor`, `TicketSystem.Tests`  
**Review Focus:** Code quality, security, best practices (all)  
**Context Provided:** Completed Core implementation (Phases 4–7); `implementation-plan.md` Phase 9 requirements

**Prompt:**
```
Execute Phase 9: perform an AI-assisted code review of the backend and 
frontend. Identify code quality issues, security concerns, and best 
practice violations. Don't fix anything yet — just report findings.
```

**AI Response Summary:**
- Reviewed backend, frontend, and test projects via parallel exploration
- Documented 52 findings in `code-review-notes.md` with severity ratings (High/Medium/Low)
- Top themes: no authentication, client-supplied `CreatedById` trusted, whitespace validation gaps, terminal ticket edits allowed, Blazor routing/UX bugs, test coverage gaps
- Identified strengths: state machine correctness, layered architecture, 28 passing integration tests

**Findings Accepted:**
- Findings recorded in `code-review-notes.md` for triage (not all implemented)

**Findings Rejected:**
- None at review stage — fixes deferred to separate triage pass

**Actions Taken:**
- Created `code-review-notes.md` with executive summary, per-area findings, cross-cutting themes, and next steps
- No code changes in this pass (report only, per prompt)

**Iteration Count:** 1

**Outcome:** Complete code review report ready for Phase 9 refinement decisions

---

### Prompt #2: Document Review Fixes and Deferred Findings

**Date:** 2026-07-16  
**Review Scope:** `review-fixes.md` (documentation of Phase 9 triage)  
**Review Focus:** Accepted fixes vs deferred findings with rationale  
**Context Provided:** `code-review-notes.md` (52 findings); four fixes already applied in codebase

**Prompt:**
```
Fill in review-fixes.md following its existing template. Use the code 
review findings from code-review-notes.md and document:
- The 4 fixes we applied (hardcoded comment author, whitespace 
  validation, terminal ticket editing, navigation reload bug) — include 
  the exact files changed for each
- The remaining findings as deferred, with reasoning: authentication 
  and CreatedById trust are out of Core scope (Stretch per spec), 
  security hardening items are beyond assessment scope, code duplication 
  and extra test tiers are deferred due to time constraints

Also fill the AI Usage Reflection section honestly based on this review 
process.
```

**AI Response Summary:**
- Filled `review-fixes.md` following the template: summary stats (4 accepted, 48 deferred, 0 rejected)
- Documented each of the 4 fixes with original issue, implementation, exact files changed, verification, and why accepted
- Grouped deferred findings into five categories (auth/identity, security hardening, code duplication, extra test tiers, UX/API polish)
- Completed Lessons Learned and AI Usage Reflection sections (what AI did well, what required human judgment, validation approach)
- Post-fix verification checklist and manual testing results

**Findings Accepted:**
- All four applied fixes documented with file lists:
  1. Comment author — `AddComment.razor`, `CreateCommentRequest.cs`, `TicketDetail.razor`
  2. Whitespace validation — `NotWhitespaceAttribute.cs`, DTOs, `TicketService.cs`, `CommentService.cs`
  3. Terminal ticket editing — `TicketService.cs`, `TicketDetail.razor`, `EditTicket.razor`
  4. Navigation reload — `TicketDetail.razor`, `EditTicket.razor`

**Findings Rejected:**
- None — remaining 48 findings documented as deferred with explicit rationale (Stretch scope, assessment scope, time constraints)

**Actions Taken:**
- Completed `review-fixes.md` (361 lines) with accepted fixes, deferred groups, category summary, test status, metrics, and sign-off

**Iteration Count:** 1

**Outcome:** Phase 9 triage fully documented — reviewers can see what was fixed, what was deferred, and why
