# Documentation Prompts

## Purpose
Document all AI prompts used for generating documentation (README, comments, technical docs).

---

## Prompt Template

### Prompt #[Number]: [Brief Description]

**Date:** [Date]  
**Document:** [Which document was being created/updated]  
**Purpose:** [What the documentation is for]  
**Context Provided:** [Context given to AI]

**Prompt:**
```
[Exact or summarized prompt text]
```

**AI Response Summary:**
[Documentation generated]

**What Was Accepted:**
- [Content used from AI]

**What Was Changed:**
- [How documentation was personalized/modified]

**What Was Rejected:**
- [Content not used]

**Personalization Done:**
[How you made it your own]

**Iteration Count:** [Number]

---

## Documentation Prompts Log

[Document each documentation prompt]

### Prompt #1: Fill Planning Docs from Implemented Code

**Date:** 2026-07-15
**Context Provided:** Fully implemented backend (entities, controllers, 
state machine) and frontend (Blazor pages, services)

**Prompt:**
Fill in data-model.md, api-contract.md, and design-notes.md based on the 
actual implemented code — document the real entities, endpoints, and 
architecture as built.

**AI Response Summary:**
- data-model.md: Documented User/Ticket/Comment with real column types, 
  Fluent API constraints, delete behaviors, the 5-user seed table, 
  migration name, DTO mapping notes, and state machine alignment with 
  test coverage.
- api-contract.md: Documented all 9 endpoints with real request/response 
  shapes, camelCase JSON, actual error formats from 
  ExceptionHandlingMiddleware, CORS origins, Swagger, and dev URLs.
- design-notes.md: Architecture diagram, project structure, tech 
  rationale, Blazor pages/components/service layer, backend layering, 
  validation/error handling/testing summary (28 tests), security notes, 
  and known Stretch gaps.

**What Was Accepted:** All three docs accepted as accurate — verified 
content matches actual source in src/TicketSystem.Api, 
src/TicketSystem.Blazor, tests/TicketSystem.Tests

**What Was Changed:** None
**What Was Rejected:** None
**Iteration Count:** 1
**Outcome:** All planning artifacts now reflect real implementation, 
no placeholder text remaining
