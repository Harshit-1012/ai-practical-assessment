# Database Schema Summary

## Migration: InitialCreate
**Created:** 2026-07-14  
**Migration File:** 20260714093803_InitialCreate.cs

## Tables Created

### 1. Users
Primary table for all users in the system (seeded data only, no CRUD in Core).

**Columns:**
- `Id` (int, PK, Identity) - Auto-incrementing primary key
- `Name` (nvarchar(200), NOT NULL) - User's full name
- `Email` (nvarchar(320), NOT NULL, UNIQUE) - User's email address
- `Role` (nvarchar(50), NOT NULL) - User role: Admin, Agent, User

**Constraints:**
- Primary Key: PK_Users (Id)
- Unique Index: IX_Users_Email (Email)

---

### 2. Tickets
Core table for support tickets.

**Columns:**
- `Id` (int, PK, Identity) - Auto-incrementing primary key
- `Title` (nvarchar(200), NOT NULL) - Brief ticket title
- `Description` (nvarchar(max), maxLength 5000, NOT NULL) - Detailed description
- `Priority` (nvarchar(50), NOT NULL) - Priority level: Low, Medium, High, Critical
- `Status` (nvarchar(50), NOT NULL) - Current status: Open, InProgress, Resolved, Closed, Cancelled
- `AssignedToId` (int, NULL) - Foreign key to Users (assigned agent)
- `CreatedById` (int, NOT NULL) - Foreign key to Users (ticket creator)
- `CreatedAt` (datetime2, NOT NULL) - Creation timestamp
- `UpdatedAt` (datetime2, NOT NULL) - Last update timestamp

**Constraints:**
- Primary Key: PK_Tickets (Id)
- Foreign Key: FK_Tickets_Users_AssignedToId → Users.Id (ON DELETE SET NULL)
- Foreign Key: FK_Tickets_Users_CreatedById → Users.Id (ON DELETE RESTRICT)
- Indexes: IX_Tickets_AssignedToId, IX_Tickets_CreatedById

---

### 3. Comments
Table for ticket comments and conversation history.

**Columns:**
- `Id` (int, PK, Identity) - Auto-incrementing primary key
- `TicketId` (int, NOT NULL) - Foreign key to Tickets
- `Message` (nvarchar(2000), NOT NULL) - Comment message text
- `CreatedById` (int, NOT NULL) - Foreign key to Users (comment author)
- `CreatedAt` (datetime2, NOT NULL) - Comment creation timestamp

**Constraints:**
- Primary Key: PK_Comments (Id)
- Foreign Key: FK_Comments_Tickets_TicketId → Tickets.Id (ON DELETE CASCADE)
- Foreign Key: FK_Comments_Users_CreatedById → Users.Id (ON DELETE RESTRICT)
- Indexes: IX_Comments_TicketId, IX_Comments_CreatedById

---

## Relationships

### User Relationships
- **User 1 → Many Tickets (CreatedBy):** Each user can create multiple tickets. Cannot delete user if they created tickets (RESTRICT).
- **User 1 → Many Tickets (AssignedTo):** Each user can be assigned multiple tickets. Deleting user sets AssignedToId to NULL (SET NULL).
- **User 1 → Many Comments (CreatedBy):** Each user can author multiple comments. Cannot delete user if they created comments (RESTRICT).

### Ticket Relationships
- **Ticket 1 → Many Comments:** Each ticket can have multiple comments. Deleting ticket deletes all comments (CASCADE).

---

## Seed Data

Five users are automatically seeded when migration is applied:

1. **Admin User** (admin@ticketsystem.com) - Role: Admin
2. **Support Agent** (agent@ticketsystem.com) - Role: Agent
3. **Regular User** (user@ticketsystem.com) - Role: User
4. **Jane Smith** (jane.smith@ticketsystem.com) - Role: Agent
5. **Bob Johnson** (bob.johnson@ticketsystem.com) - Role: User

---

## Database Configuration

**Connection String Format (LocalDB):**
```
Server=(localdb)\\mssqllocaldb;Database=TicketSystemDb;Trusted_Connection=true;TrustServerCertificate=true
```

**Location in Code:**
- `appsettings.json` - ConnectionStrings:DefaultConnection

---

## State Machine Rules (Enforced in Application Logic)

The database allows any status value, but the application enforces these transitions:

**Valid Transitions:**
- Open → InProgress
- Open → Cancelled
- InProgress → Resolved
- InProgress → Cancelled
- Resolved → Closed

**Terminal States:**
- Closed (no transitions allowed)
- Cancelled (no transitions allowed)

---

## How to Apply Migration

### First Time Setup
```bash
cd src/TicketSystem.Api
dotnet ef database update
```

### Verify Migration Applied
```bash
dotnet ef migrations list
```

Should show:
```
20260714093803_InitialCreate (Applied)
```

---

## Notes

- All string fields use nvarchar for Unicode support
- DateTime fields use datetime2 for better precision
- Foreign key indexes are automatically created by SQL Server
- Unique constraint on User.Email ensures no duplicate emails
- Cascade delete on Comments ensures orphaned comments are removed
- Restrict delete on User references ensures data integrity
