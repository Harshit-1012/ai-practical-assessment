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

[Document each planning prompt using the template above]

### Prompt #1: [Title]
[Details]

### Prompt #2: [Title]
[Details]

[Continue...]
