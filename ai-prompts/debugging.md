# Debugging Prompts

## Purpose
Document all AI prompts used during debugging and issue resolution.

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
Identified 4 compounding root causes: CORS middleware registered after HTTPS redirect (causing silent CORS failures), a @bind:after pattern in SearchFilter that could re-trigger loading state, missing HttpClient timeout (causing long hangs on failure), and the general case of the API simply not running.

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





## Issue 2: Test provider conflict (SQL Server + SQLite)

### Problem
All 28 integration tests failed immediately with 
InvalidOperationException about multiple database providers registered.

### How I Investigated
Read the error message, recognized it as an EF Core service registration conflict rather than a logic bug, and asked Cursor to fix the test factory setup.

### How AI Helped
Identified that WebApplicationFactory wasn't properly removing the 
existing SQL Server DbContextOptions registration before adding the 
SQLite one for tests, causing both to be registered simultaneously.

### What I Validated
Re-ran the full test suite after the fix — confirmed all 28 tests now 
pass (0 failed).

### Final Fix
Corrected the WebApplicationFactory to explicitly remove the SQL Server ServiceDescriptor before registering the in-memory SQLite provider.


### Prompt #2: Fix Test Provider Conflict

**Date:** 2026-07-15
**Context Provided:** Test run showing all 28 tests failing with 
InvalidOperationException about database providers

**Prompt:**
All 28 tests are failing with: "System.InvalidOperationException: 
Services for database providers 'Microsoft.EntityFrameworkCore.SqlServer' 
and 'Microsoft.EntityFrameworkCore.Sqlite' have been registered in the 
service provider. Only a single database provider can be registered..."

Fix the WebApplicationFactory test setup so it properly removes/replaces 
the SQL Server DbContext registration from Program.cs with the in-memory 
SQLite one for tests — likely need to remove the existing 
ServiceDescriptor for DbContextOptions<AppDbContext> before adding the 
SQLite one. Show me the corrected test factory code.

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