# Planning Prompts

## Purpose
Document all AI prompts used during the planning and requirement analysis phases.

---

## Prompt Template

### Prompt #[Number]: [Brief Description]

**Date:** [Date]  
**Phase:** Planning/Requirements  
**Context Provided:** [What context was given to AI]

**Prompt:**
```
[Exact or summarized prompt text]
```

**AI Response Summary:**
[Key points from AI's response]

**What Was Accepted:**
- [Points or code accepted from AI response]

**What Was Changed:**
- [Modifications made to AI output]

**What Was Rejected:**
- [Parts not used and why]

**Iteration Count:** [How many back-and-forth exchanges]

**Outcome:** [Final result from this prompt chain]

---

## Example Prompts (Delete when adding real prompts)

### Prompt #1: Extract Requirements from Spec

**Date:** 2026-07-01  
**Phase:** Requirements Analysis  
**Context Provided:** Provided spec.md file content

**Prompt:**
```
Read this specification document and extract all functional requirements 
for the Support Ticket Management System. Organize them by feature area 
(Ticket Management, Comment System, State Machine, Search/Filter).
```

**AI Response Summary:**
AI extracted requirements and organized them into categories. Identified 
core features vs. stretch features. Listed state machine transitions.

**What Was Accepted:**
- Requirement categorization structure
- List of core CRUD operations
- State machine transition rules

**What Was Changed:**
- Reworded some requirements in my own words
- Added specific validation rules not explicitly in AI response
- Clarified edge cases

**What Was Rejected:**
- Some stretch feature assumptions that weren't in spec

**Iteration Count:** 2  
**Outcome:** Created requirements-analysis.md with clear functional requirements

---

## Prompts Log



### Prompt #1: Project Setup + Cursor Context (Phase 1 & 2)

**Date:** 2026-07-14
**Phase:** Planning/Setup
**Context Provided:** implementation-plan.md (11-phase plan)

**Prompt:** Execute Phase 1 (Project Setup) and Phase 2 (Cursor Setup) from
implementation-plan.md. Create the .NET solution with 3 projects, the
full assessment folder structure, and the Cursor context files.

**AI Response Summary:**
Created TicketSystem.Api, TicketSystem.Blazor, TicketSystem.Tests 
projects; full folder/file skeleton (31 files); .cursorrules; 
tool-specific/cursor-workflow/ files.

**What Was Accepted:**
- Solution structure and all 3 projects
- Full documentation folder skeleton

**What Was Changed:**
- spec.md was created at project root instead of 
  tool-specific/cursor-workflow/ — asked Cursor to move it to the 
  correct location

**What Was Rejected:**
- None

**Iteration Count:** 2
**Outcome:** Working solution + complete repo skeleton, git initialized 
and first commit made

---

### Prompt #2: Implementation Plan Generation

**Date:** 2026-07-14
**Phase:** Planning
**Context Provided:** spec.md, earlier Plan-mode phase breakdown

**Prompt:**
Using spec.md and the plan file as reference, create
implementation-plan.md with: Overview, Task Breakdown, Milestones,
AI Usage Plan, Risks, Mitigation.

**AI Response Summary:**
Generated full 11-phase plan with day-wise milestones, AI usage strategy 
per lifecycle phase, and a 10-item risk/mitigation table.

**What Was Accepted:**
- Overall 11-phase structure and sequencing
- Risk/mitigation table

**What Was Changed:**
- Corrected an error stating documentation alone was 60% of evaluation 
  (spec actually weights Part B — full project + artifacts — at 60%)
- Updated project location path to match actual working directory

**What Was Rejected:**
- None

**Iteration Count:** 2
**Outcome:** Finalized implementation-plan.md

---

### Prompt #3: Requirements Analysis Template

**Date:** 2026-07-14
**Phase:** Requirements Analysis
**Context Provided:** spec.md

**Prompt:** Create requirements-analysis.md with Functional/Non-Functional
Requirements and Edge Cases pre-filled based on the spec, leaving
Understanding/Assumptions/Clarifications empty for me to fill personally.

**AI Response Summary:**
Structured template with detailed Functional/Non-Functional Requirements 
and Edge Cases broken down by category (state machine, validation, 
search, database).

**What Was Accepted:**
- Functional and Non-Functional Requirements sections as-is
- Edge Cases categorization

**What Was Changed:**
- N/A (AI-generated sections kept as-is)

**What Was Rejected:**
- None

**Iteration Count:** 1
**Outcome:** Requirements-analysis.md template created; My Understanding, 
Assumptions, and Clarifications written personally afterward (not 
AI-generated)

