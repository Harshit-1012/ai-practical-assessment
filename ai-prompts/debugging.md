# Debugging Prompts

## Purpose
Document all AI prompts used during debugging and issue resolution.

---

## Prompt Template

### Prompt #[Number]: [Brief Description of Issue]

**Date:** [Date]  
**Issue:** [Brief issue description]  
**Error Message:** [If applicable]  
**Context Provided:** [Code/logs shared with AI]

**Prompt:**
```
[Exact or summarized prompt text]
```

**AI Response Summary:**
[AI's diagnosis and suggested fixes]

**What Was Helpful:**
- [Useful suggestions from AI]

**What Was Not Helpful:**
- [Incorrect or unhelpful suggestions]

**Actual Root Cause:**
[What you discovered the real issue was]

**Final Fix:**
[How you actually fixed it]

**AI Accuracy:** [High/Medium/Low]

**Iteration Count:** [Number]

---

## Debugging Prompts Log

[Document each debugging prompt]

## Issue 1: Ticket list stuck on "Loading tickets..." indefinitely

### Problem
After Phase 6 frontend build, the ticket list page loaded but never 
displayed tickets — infinite loading spinner.

### How I Investigated
Asked Cursor to check browser console/network errors and verify API/CORS 
configuration rather than guessing at a fix.

### How AI Helped
Identified 4 compounding root causes: CORS middleware registered after 
HTTPS redirect (causing silent CORS failures), a @bind:after pattern in 
SearchFilter that could re-trigger loading state, missing HttpClient 
timeout (causing long hangs on failure), and the general case of the API 
simply not running.

### What I Validated
Restarted both apps with the http launch profile, opened browser dev 
tools, confirmed the CORS preflight request returned 204 with the 
correct Access-Control-Allow-Origin header, and confirmed tickets 
actually rendered in the UI.

### Final Fix
Reordered middleware (CORS before HTTPS redirect, HTTPS redirect 
disabled in Development), added explicit allowed origins, replaced 
@bind:after with value+@onchange in SearchFilter, added a 30s HttpClient 
timeout.
