# Design Prompts

## Purpose
Document all AI prompts used during the design phase (architecture, data model, API design, UI design).

---

## Design Prompts Log


### Prompt #1: Database Schema, Entities & Migration (Phase 4)

**Date:** 2026-07-14
**Design Area:** Database
**Context Provided:** implementation-plan.md (Phase 4 task list), spec.md 
(entity field requirements)

**Prompt:**

Execute Phase 4 (Database Design & Setup) from implementation-plan.md:
create the User, Ticket, Comment entities, AppDbContext with Fluent API
configuration, initial EF Core migration, and seed data for 3-5 sample
users.

**AI Response Summary:**
Created 3 entity classes with relationships; AppDbContext with Fluent 
API constraints (Title: 200 chars, Description: 5000, Message: 2000) 
and cascade/restrict/set-null delete behaviors; InitialCreate migration; 
5 seeded users; connection string in appsettings.json (LocalDB); schema 
documentation.

**What Was Accepted:**
- Entity field definitions and lengths
- Delete behaviors (verified correct: Ticket→Comment Cascade, 
  User→Ticket CreatedBy Restrict, User→Ticket AssignedTo SetNull)
- Migration structure and seed data

**What Was Changed:**
- None needed — reviewed delete behaviors before approving and found 
  them already correctly configured

**What Was Rejected:**
- None

**Iteration Count:** 1

**Artifacts Created:**
- src/TicketSystem.Api/Models/User.cs, Ticket.cs, Comment.cs
- src/TicketSystem.Api/Data/AppDbContext.cs
- src/TicketSystem.Api/Migrations/20260714093803_InitialCreate.cs
- database/schema-or-migrations/SCHEMA_SUMMARY.md
- database/seed-data/seed_users.sql

### Prompt #2: Persistence Verification (Phase 4 — Critical Test)

**Date:** 2026-07-15
**Design Area:** Database
**Context Provided:** Working API endpoints (Phase 5 complete)

**Prompt:** 
Create a test ticket via the API, stop the API, restart it, and confirm
the ticket still exists. Document this in database/setup-notes.md.

**AI Response Summary:**
Created ticket #3 via POST /api/tickets, stopped and restarted the API 
process, then confirmed via GET /api/tickets/3 that the ticket returned 
with identical data. Documented full procedure in setup-notes.md.

**What Was Accepted:**
- Full test procedure and PASS result as-is

**What Was Changed:**
- None

**What Was Rejected:**
- None

**Iteration Count:** 1
**Outcome:** PASS — confirmed data persists in SQL Server LocalDB across 
API restarts, not stored in-memory.