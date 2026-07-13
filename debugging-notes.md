# Debugging Notes

## Purpose

This document captures issues encountered during development, how they were investigated, how AI assisted in diagnosis, what was validated manually, and the final fixes applied.

---

## Issue Template

### Issue #[Number]: [Brief Title]

**Date:** [Date]  
**Phase:** [Planning/Implementation/Testing/etc.]  
**Severity:** [Critical/High/Medium/Low]

**Problem Description:**
[Describe the issue: What went wrong? What was the expected behavior vs actual behavior?]

**How I Investigated:**
[Steps taken to diagnose the issue:
- Error messages reviewed
- Logs examined
- Breakpoints set
- Database queries run
- API endpoints tested
- etc.]

**How AI Helped:**
[What prompts were used? What AI suggested? Was it helpful?]

**What I Validated Manually:**
[What testing/verification you did yourself to understand the issue]

**Root Cause:**
[What was the underlying cause of the issue?]

**Fix Applied:**
[What code changes or configuration changes were made?]

**Verification:**
[How you confirmed the fix works]

**Lessons Learned:**
[What you learned from this issue]

---

## Example Issue (Delete this section when adding real issues)

### Issue #1: State Machine Allowing Invalid Transition

**Date:** 2026-07-03  
**Phase:** Testing  
**Severity:** Critical

**Problem Description:**
During integration testing, discovered that tickets in "Open" status could be transitioned directly to "Closed" status, which violates the state machine rules. Expected a 400 Bad Request error, but the API returned 200 OK.

**How I Investigated:**
1. Ran the failing integration test and captured the response
2. Set breakpoint in `TicketStateMachine.CanTransition()` method
3. Stepped through the code to see which condition was being evaluated
4. Reviewed the state machine transition logic in TicketStateMachine.cs
5. Checked if the validation was being called in the service layer

**How AI Helped:**
Prompt: "Review this state machine code and identify why it's allowing Open → Closed transition"
AI identified that the switch statement was missing a case for the "Open" status, causing it to fall through to the default case which returned true.

**What I Validated Manually:**
- Tested the transition via Postman to confirm API behavior
- Checked database to see if invalid status was persisted
- Reviewed other transition combinations to see if similar bugs existed

**Root Cause:**
Incomplete switch statement in TicketStateMachine.cs - the case for TicketStatus.Open was missing, so it fell through to the default case which returned true for all transitions.

**Fix Applied:**
Added the missing case statement:
```csharp
case TicketStatus.Open:
    return newStatus == TicketStatus.InProgress || newStatus == TicketStatus.Cancelled;
```

**Verification:**
- Re-ran all state machine integration tests - all passed
- Manually tested invalid transitions via API - all returned 400
- Verified valid transitions still work correctly

**Lessons Learned:**
- Always use explicit handling for all enum values in switch statements
- Consider using switch expressions with exhaustive pattern matching
- Integration tests caught this before it reached production
- AI was helpful in quickly identifying the logic gap

---

## Issues Log

[Document each issue you encounter using the template above]
