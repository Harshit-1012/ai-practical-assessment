# Implementation Prompts

## Purpose
Document all AI prompts used during implementation (backend, frontend, database, state machine).

---

## Prompt Template

### Prompt #[Number]: [Brief Description]

**Date:** [Date]  
**Component:** [Backend/Frontend/Database/Shared]  
**File(s) Affected:** [List files created/modified]  
**Context Provided:** [Context given to AI]

**Prompt:**
```
[Exact or summarized prompt text]
```

**AI Response Summary:**
[What AI generated]

**Code Generated:** [Lines of code or percentage]

**What Was Accepted:**
- [Code/logic accepted as-is]

**What Was Changed:**
- [Modifications made and why]

**What Was Rejected:**
- [Code not used and why]

**Testing Done:**
[How the generated code was tested]

**Iteration Count:** [Number]

**Final Outcome:**
[Working feature/component]

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