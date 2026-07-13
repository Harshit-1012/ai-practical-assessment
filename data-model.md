# Data Model

## Entities

### User

**Purpose:** Represents system users who can create, be assigned to, and comment on tickets.

**Fields:**
- `Id` (int, PK): Primary key, auto-increment
- `Name` (string, max 200): User's full name
- `Email` (string, max 320): User's email address (unique)
- `Role` (string, max 50): User's role (Admin, Agent, User)

**Constraints:**
- Email must be unique
- Role must be one of the predefined values

**Notes:**
- Users are seeded in database, no CRUD operations in Core
- Used for ticket assignment and tracking creators

---

### Ticket

**Purpose:** Core entity representing a support ticket.

**Fields:**
- `Id` (int, PK): Primary key, auto-increment
- `Title` (string, max 200): Brief description of the issue
- `Description` (string, max 5000): Detailed description
- `Priority` (string, max 50): Priority level (Low, Medium, High, Critical)
- `Status` (string, max 50): Current ticket status (Open, InProgress, Resolved, Closed, Cancelled)
- `AssignedToId` (int, FK, nullable): References User.Id for assigned agent
- `CreatedById` (int, FK): References User.Id for ticket creator
- `CreatedAt` (DateTime): Timestamp when ticket was created
- `UpdatedAt` (DateTime): Timestamp when ticket was last updated

**Relationships:**
- One-to-many with User (AssignedTo)
- One-to-many with User (CreatedBy)
- One-to-many with Comment

**Constraints:**
- Title is required
- Description is required
- CreatedById is required and must reference existing user
- AssignedToId must reference existing user if not null

---

### Comment

**Purpose:** Represents a comment added to a ticket.

**Fields:**
- `Id` (int, PK): Primary key, auto-increment
- `TicketId` (int, FK): References Ticket.Id
- `Message` (string, max 2000): Comment text
- `CreatedById` (int, FK): References User.Id for comment author
- `CreatedAt` (DateTime): Timestamp when comment was created

**Relationships:**
- Many-to-one with Ticket
- Many-to-one with User (CreatedBy)

**Constraints:**
- Message is required
- TicketId is required and must reference existing ticket
- CreatedById is required and must reference existing user

---

## Entity Relationships

```
┌────────────┐
│   User     │
│────────────│
│ Id (PK)    │
│ Name       │◄────────┐
│ Email      │         │
│ Role       │         │
└────────────┘         │
      ▲                │
      │                │
      │                │
      │ CreatedBy      │ AssignedTo
      │                │
      │                │
┌─────┴──────┐         │
│   Ticket   │◄────────┘
│────────────│
│ Id (PK)    │
│ Title      │
│ Description│
│ Priority   │
│ Status     │──────── State Machine Enforced
│ AssignedTo │
│ CreatedBy  │
│ CreatedAt  │
│ UpdatedAt  │
└────────────┘
      ▲
      │
      │ TicketId
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

### Status Enum Values
- `Open`: Initial state for new tickets
- `InProgress`: Ticket is being worked on
- `Resolved`: Issue has been fixed but not yet closed
- `Closed`: Ticket is completed and closed (terminal state)
- `Cancelled`: Ticket was cancelled without resolution (terminal state)

### Valid Transitions

```
          ┌─────────┐
          │  Open   │
          └────┬────┘
               │
      ┌────────┴────────┐
      │                 │
      ▼                 ▼
┌─────────┐       ┌───────────┐
│Cancelled│       │InProgress │
└─────────┘       └─────┬─────┘
 (Terminal)             │
                   ┌────┴────┐
                   │         │
                   ▼         ▼
              ┌─────────┐  ┌─────────┐
              │Resolved │  │Cancelled│
              └────┬────┘  └─────────┘
                   │        (Terminal)
                   ▼
              ┌─────────┐
              │ Closed  │
              └─────────┘
               (Terminal)
```

**Allowed Transitions:**
- Open → InProgress
- Open → Cancelled
- InProgress → Resolved
- InProgress → Cancelled
- Resolved → Closed

**Invalid Transitions (Examples):**
- Open → Resolved (must go through InProgress)
- Open → Closed (must go through InProgress and Resolved)
- Resolved → InProgress (cannot reopen resolved tickets)
- Closed → Any (terminal state)
- Cancelled → Any (terminal state)

### Business Rules

1. **New tickets start as Open**
   - When a ticket is created, status is automatically set to "Open"

2. **Terminal states cannot transition**
   - Once a ticket reaches "Closed" or "Cancelled", no further status changes are allowed

3. **Only valid transitions are allowed**
   - API must enforce state machine rules
   - Backend validation prevents invalid transitions
   - Frontend should only show valid transition buttons

4. **Status changes update UpdatedAt**
   - Any status change automatically updates the UpdatedAt timestamp

---

## Database Indexes

### Primary Keys
- User.Id (clustered)
- Ticket.Id (clustered)
- Comment.Id (clustered)

### Foreign Keys (Automatically indexed by SQL Server)
- Ticket.AssignedToId → User.Id
- Ticket.CreatedById → User.Id
- Comment.TicketId → Ticket.Id
- Comment.CreatedById → User.Id

### Unique Constraints
- User.Email (unique)

### Recommended Indexes (Optional for Core)
- Ticket.Status (for filter queries)
- Ticket.CreatedAt (for sorting)
- Comment.TicketId (if not automatically indexed)

---

## Data Types and Validation

### String Length Constraints
| Field | Max Length | Reason |
|-------|------------|--------|
| User.Name | 200 | Reasonable name length |
| User.Email | 320 | RFC 5321 max email length |
| User.Role | 50 | Enum values |
| Ticket.Title | 200 | Brief summary |
| Ticket.Description | 5000 | Detailed explanation |
| Ticket.Priority | 50 | Enum values |
| Ticket.Status | 50 | Enum values |
| Comment.Message | 2000 | Substantial comment |

### Enum Values

**Priority:**
- Low
- Medium
- High
- Critical

**Status:**
- Open
- InProgress
- Resolved
- Closed
- Cancelled

**Role:**
- Admin
- Agent
- User

---

## Seed Data

### Sample Users
```json
[
  {
    "Id": 1,
    "Name": "Admin User",
    "Email": "admin@ticketsystem.com",
    "Role": "Admin"
  },
  {
    "Id": 2,
    "Name": "Support Agent",
    "Email": "agent@ticketsystem.com",
    "Role": "Agent"
  },
  {
    "Id": 3,
    "Name": "Regular User",
    "Email": "user@ticketsystem.com",
    "Role": "User"
  }
]
```
