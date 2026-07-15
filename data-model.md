# Data Model

## Overview

The Support Ticket Management System uses a relational model with three entities (`User`, `Ticket`, `Comment`) persisted in **SQL Server** via **Entity Framework Core 9**. Enum-like values (status, priority, role) are stored as strings in the database and validated in application code using C# enums and extension helpers.

**Database:** `TicketSystemDb` on SQL Server LocalDB  
**Migration:** `20260714093803_InitialCreate`  
**DbContext:** `AppDbContext` (`src/TicketSystem.Api/Data/AppDbContext.cs`)

---

## Entities

### User

**Purpose:** Represents system users who create tickets, are assigned to tickets, and author comments.

**Table:** `Users`

| Column | Type | Constraints |
|--------|------|-------------|
| `Id` | `int` | PK, identity |
| `Name` | `nvarchar(200)` | Required |
| `Email` | `nvarchar(320)` | Required, unique index |
| `Role` | `nvarchar(50)` | Required |

**Valid roles:** `Admin`, `Agent`, `User` (validated at application level; stored as string)

**API exposure:** Read-only (`GET /api/users`, `GET /api/users/{id}`). No create/update/delete endpoints in Core.

**Navigation properties:** None defined on the entity; referenced by `Ticket` and `Comment`.

---

### Ticket

**Purpose:** Core entity representing a support request.

**Table:** `Tickets`

| Column | Type | Constraints |
|--------|------|-------------|
| `Id` | `int` | PK, identity |
| `Title` | `nvarchar(200)` | Required |
| `Description` | `nvarchar(5000)` | Required |
| `Priority` | `nvarchar(50)` | Required |
| `Status` | `nvarchar(50)` | Required |
| `AssignedToId` | `int` | FK → `Users.Id`, nullable |
| `CreatedById` | `int` | FK → `Users.Id`, required |
| `CreatedAt` | `datetime2` | Required |
| `UpdatedAt` | `datetime2` | Required |

**Valid priorities:** `Low`, `Medium`, `High`, `Critical` (`TicketPriority` enum)

**Valid statuses:** `Open`, `InProgress`, `Resolved`, `Closed`, `Cancelled` (`TicketStatus` enum)

**Navigation properties:**
- `AssignedTo` → `User` (optional)
- `CreatedBy` → `User` (required)
- `Comments` → collection of `Comment`

**Business rules (implemented in `TicketService`):**
- New tickets are always created with status `Open` and `CreatedAt`/`UpdatedAt` set to UTC now.
- `Title` and `Description` are trimmed on create/update.
- `AssignedToId` and `CreatedById` must reference existing users when provided/required.
- Status changes go through `TicketStateMachine` only (not via the update endpoint).

**Delete behaviors:**
- `AssignedTo` FK: `SetNull` (deleting a user clears assignment)
- `CreatedBy` FK: `Restrict` (cannot delete a user who created tickets)
- `Comments` cascade delete when a ticket is deleted

---

### Comment

**Purpose:** A message attached to a ticket for collaboration and audit.

**Table:** `Comments`

| Column | Type | Constraints |
|--------|------|-------------|
| `Id` | `int` | PK, identity |
| `TicketId` | `int` | FK → `Tickets.Id`, required |
| `Message` | `nvarchar(2000)` | Required |
| `CreatedById` | `int` | FK → `Users.Id`, required |
| `CreatedAt` | `datetime2` | Required |

**Navigation properties:**
- `Ticket` → `Ticket` (required)
- `CreatedBy` → `User` (required)

**Business rules (implemented in `CommentService`):**
- `Message` is trimmed on create.
- `TicketId` and `CreatedById` must reference existing records.
- Comments are append-only (no update/delete endpoints in Core).
- `CreatedAt` is set to UTC now on create.

**Delete behaviors:**
- `Ticket` FK: `Cascade` (comments removed with ticket)
- `CreatedBy` FK: `Restrict`

---

## Entity Relationships

```
┌────────────┐
│   User     │
│────────────│
│ Id (PK)    │
│ Name       │◄────────┐
│ Email (UK) │         │
│ Role       │         │
└────────────┘         │
      ▲                │
      │ CreatedBy      │ AssignedTo (nullable)
      │                │
┌─────┴──────┐         │
│   Ticket   │◄────────┘
│────────────│
│ Id (PK)    │
│ Title      │
│ Description│
│ Priority   │
│ Status     │──────── State machine enforced (TicketStateMachine)
│ AssignedTo │
│ CreatedBy  │
│ CreatedAt  │
│ UpdatedAt  │
└────────────┘
      ▲
      │ TicketId (cascade delete)
      │
┌─────┴──────┐
│  Comment   │
│────────────│
│ Id (PK)    │
│ TicketId   │
│ Message    │
│ CreatedBy  │──────► User
│ CreatedAt  │
└────────────┘
```

---

## State Machine Rules

Implemented in `TicketStateMachine` (`src/TicketSystem.Api/Services/TicketStateMachine.cs`). All status changes via `PUT /api/tickets/{id}/status` call `ValidateTransition`, which throws `InvalidTransitionException` on invalid moves.

### Status values

| Status | Description |
|--------|-------------|
| `Open` | Initial state for new tickets |
| `InProgress` | Actively being worked |
| `Resolved` | Fix applied, awaiting closure |
| `Closed` | Terminal — completed |
| `Cancelled` | Terminal — abandoned |

### Transition diagram

```
          ┌─────────┐
          │  Open   │
          └────┬────┘
               │
      ┌────────┴────────┐
      │                 │
      ▼                 ▼
┌───────────┐     ┌─────────┐
│ Cancelled │     │InProgress│
└───────────┘     └────┬────┘
  (terminal)           │
                 ┌─────┴─────┐
                 │           │
                 ▼           ▼
            ┌─────────┐  ┌───────────┐
            │Resolved │  │ Cancelled │
            └────┬────┘  └───────────┘
                 │         (terminal)
                 ▼
            ┌─────────┐
            │ Closed  │
            └─────────┘
             (terminal)
```

### Allowed transitions

| From | To |
|------|-----|
| `Open` | `InProgress`, `Cancelled` |
| `InProgress` | `Resolved`, `Cancelled` |
| `Resolved` | `Closed` |
| `Closed` | *(none)* |
| `Cancelled` | *(none)* |

### Enforcement points

1. **API:** `TicketService.ChangeStatusAsync` → `ITicketStateMachine.ValidateTransition`
2. **UI:** `TicketWorkflowService` mirrors rules for enabling/disabling transition buttons (presentation only; API is authoritative)
3. **Tests:** 16 integration tests in `StateMachineTransitionTests` cover all valid and required invalid transitions

---

## Database Indexes and Constraints

### Primary keys
- `Users.Id`, `Tickets.Id`, `Comments.Id` (clustered, identity)

### Unique constraints
- `Users.Email` (unique index via Fluent API)

### Foreign keys (SQL Server indexes FK columns automatically)
- `Tickets.AssignedToId` → `Users.Id`
- `Tickets.CreatedById` → `Users.Id`
- `Comments.TicketId` → `Tickets.Id`
- `Comments.CreatedById` → `Users.Id`

### Additional indexes
No custom indexes beyond the unique email constraint. List queries order by `Ticket.CreatedAt` descending; status/keyword filters use in-memory predicate evaluation via LINQ `Where`.

---

## Seed Data

Five users are seeded via `HasData` in `AppDbContext.OnModelCreating` (migration `InitialCreate`):

| Id | Name | Email | Role |
|----|------|-------|------|
| 1 | Admin User | admin@ticketsystem.com | Admin |
| 2 | Support Agent | agent@ticketsystem.com | Agent |
| 3 | Regular User | user@ticketsystem.com | User |
| 4 | Jane Smith | jane.smith@ticketsystem.com | Agent |
| 5 | Bob Johnson | bob.johnson@ticketsystem.com | User |

Tickets and comments are not seeded; they are created at runtime via the API or UI.

---

## DTO Mapping (API layer)

Entities are not returned directly. Services map to DTOs:

| DTO | Used for |
|-----|----------|
| `TicketResponseDto` | Ticket list, detail, create, update, status change |
| `CreateTicketDto` | `POST /api/tickets` |
| `UpdateTicketDto` | `PUT /api/tickets/{id}` |
| `ChangeTicketStatusDto` | `PUT /api/tickets/{id}/status` |
| `CommentResponseDto` | Comment list and create |
| `CreateCommentDto` | `POST /api/tickets/{ticketId}/comments` |
| `UserResponseDto` | User list and detail |

`TicketResponseDto` includes denormalized `AssignedToName` and `CreatedByName` for display. Comments are included only on `GET /api/tickets/{id}`, not on the list endpoint.

---

## Test Database

Integration tests (`TicketSystem.Tests`) use **in-memory SQLite** via `CustomWebApplicationFactory` when `ASPNETCORE_ENVIRONMENT=Testing`. `Program.cs` skips SQL Server registration in that environment; the factory registers `UseSqlite` instead. Schema is created with `EnsureCreatedAsync()`; tickets/comments are reset between tests while seeded users remain.
