# Database Setup Notes

## Database Information

**Database Type:** SQL Server  
**ORM:** Entity Framework Core 9.0  
**Development Database:** SQL Server LocalDB (recommended)  
**Production Database:** SQL Server 2019+ (if deploying)

---

## Prerequisites

### Windows (LocalDB - Recommended for Development)
LocalDB comes with Visual Studio or SQL Server Express
- Check if installed: `SqlLocalDB.exe info`
- If not installed: Download SQL Server Express or Visual Studio

### SQL Server Full Instance
- SQL Server 2019 or later
- SQL Server Management Studio (SSMS) for database management

---

## Connection String Configuration

### Option 1: LocalDB (Recommended)

**Location:** `src/TicketSystem.Api/appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TicketSystemDb;Trusted_Connection=true;TrustServerCertificate=true"
  }
}
```

### Option 2: SQL Server with Windows Authentication

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TicketSystemDb;Trusted_Connection=true;TrustServerCertificate=true"
  }
}
```

### Option 3: SQL Server with SQL Authentication

**⚠️ Never commit passwords to source control!**

Use User Secrets for local development:

```bash
cd src/TicketSystem.Api
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=TicketSystemDb;User Id=your_username;Password=your_password;TrustServerCertificate=true"
```

---

## Database Setup Steps

### Step 1: Install EF Core Tools

```bash
dotnet tool install --global dotnet-ef
```

Or update if already installed:
```bash
dotnet tool update --global dotnet-ef
```

### Step 2: Verify Installation

```bash
dotnet ef
```

Should display EF Core command help.

### Step 3: Create Database and Run Migrations

```bash
cd src/TicketSystem.Api
dotnet ef database update
```

Expected output:
```
Build started...
Build succeeded.
Applying migration '20260701_InitialCreate'.
Applying migration '20260701_SeedUsers'.
Done.
```

### Step 4: Verify Database Creation

**Using LocalDB:**
```bash
SqlLocalDB.exe info mssqllocaldb
```

**Using SSMS:**
1. Connect to `(localdb)\mssqllocaldb` or your server
2. Expand Databases
3. Find `TicketSystemDb`
4. Expand Tables - should see: Users, Tickets, Comments

---

## Database Schema

### Tables Created

**1. Users**
```sql
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(200) NOT NULL,
    Email NVARCHAR(320) NOT NULL UNIQUE,
    Role NVARCHAR(50) NOT NULL
);
```

**2. Tickets**
```sql
CREATE TABLE Tickets (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
    Priority NVARCHAR(50) NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    AssignedToId INT NULL,
    CreatedById INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NOT NULL,
    CONSTRAINT FK_Tickets_Users_Assigned FOREIGN KEY (AssignedToId) REFERENCES Users(Id),
    CONSTRAINT FK_Tickets_Users_Created FOREIGN KEY (CreatedById) REFERENCES Users(Id)
);
```

**3. Comments**
```sql
CREATE TABLE Comments (
    Id INT PRIMARY KEY IDENTITY(1,1),
    TicketId INT NOT NULL,
    Message NVARCHAR(MAX) NOT NULL,
    CreatedById INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    CONSTRAINT FK_Comments_Tickets FOREIGN KEY (TicketId) REFERENCES Tickets(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Comments_Users FOREIGN KEY (CreatedById) REFERENCES Users(Id)
);
```

---

## Seed Data

### Default Users

Three users are automatically seeded when migrations run:

1. **Admin User**
   - Name: Admin User
   - Email: admin@ticketsystem.com
   - Role: Admin

2. **Support Agent**
   - Name: Support Agent
   - Email: agent@ticketsystem.com
   - Role: Agent

3. **Regular User**
   - Name: Regular User
   - Email: user@ticketsystem.com
   - Role: User

### Seed Data Location

Seed data is configured in: `src/TicketSystem.Api/Data/AppDbContext.cs` or via migration file.

---

## Data Persistence Verification

### Critical Test: Verify Data Survives Restart

**Steps:**
1. Run the API: `dotnet run` in `src/TicketSystem.Api`
2. Create a test ticket via API or Blazor UI
3. Note the ticket ID
4. Stop the API (Ctrl+C)
5. Restart the API: `dotnet run`
6. Retrieve the ticket by ID: GET `/api/tickets/{id}`
7. Verify the ticket still exists with same data

**Expected Result:** ✓ Ticket persists across restarts

**Test Date:** [Date performed]  
**Test Result:** [PASS/FAIL]  
**Notes:** [Any observations]

---

## Migration Management

### Creating New Migrations

When you modify entities:

```bash
cd src/TicketSystem.Api
dotnet ef migrations add MigrationName
```

### Applying Migrations

```bash
dotnet ef database update
```

### Rolling Back Migrations

To a specific migration:
```bash
dotnet ef database update PreviousMigrationName
```

### Removing Last Migration (if not applied)

```bash
dotnet ef migrations remove
```

### Viewing Migration History

```bash
dotnet ef migrations list
```

---

## Troubleshooting

### Issue: "Unable to create an object of type 'AppDbContext'"

**Solution:** Ensure you're in the correct directory (`src/TicketSystem.Api`) and the project builds successfully.

```bash
dotnet build
dotnet ef database update
```

---

### Issue: "A network-related or instance-specific error occurred"

**Solution:**
1. Verify SQL Server is running
2. Check connection string in `appsettings.json`
3. For LocalDB: `SqlLocalDB.exe start mssqllocaldb`

---

### Issue: "Login failed for user"

**Solution:**
- Check authentication mode in connection string
- For Windows Auth: Use `Trusted_Connection=true`
- For SQL Auth: Verify username/password
- Ensure user has permission to create databases

---

### Issue: "Cannot open database 'TicketSystemDb' requested by the login"

**Solution:**
Database doesn't exist yet. Run migrations:
```bash
dotnet ef database update
```

---

### Issue: "Certificate chain validation error"

**Solution:**
Add `TrustServerCertificate=true` to connection string.

---

## Database Management

### Viewing Data with SSMS

1. Open SQL Server Management Studio
2. Connect to: `(localdb)\mssqllocaldb` or your server
3. Navigate to: Databases → TicketSystemDb → Tables
4. Right-click table → "Select Top 1000 Rows"

### Resetting the Database

**Warning:** This deletes all data!

```bash
cd src/TicketSystem.Api
dotnet ef database drop
dotnet ef database update
```

### Exporting Seed Data

If you want to backup seed data:

```sql
SELECT * FROM Users;
SELECT * FROM Tickets;
SELECT * FROM Comments;
```

Save results as SQL INSERT statements.

---

## Alternative: SQLite (Not Recommended but Possible)

If SQL Server is unavailable, you can use SQLite for development:

1. Install NuGet package: `Microsoft.EntityFrameworkCore.Sqlite`
2. Update connection string: `Data Source=ticketsystem.db`
3. Update DbContext configuration to use SQLite provider

**Note:** This is not the intended database for this project. SQL Server is required per the specification.

---

## Connection String Security

### Do NOT Commit:
❌ Passwords in `appsettings.json`  
❌ Production connection strings  
❌ API keys or credentials  

### DO Use:
✓ `appsettings.json` for LocalDB with Windows Auth  
✓ User Secrets for local SQL Auth  
✓ Environment variables for deployed environments  
✓ Azure Key Vault or similar for production  

### .gitignore Configuration

Ensure these are in `.gitignore`:
```
appsettings.Development.json
appsettings.*.json
!appsettings.json
*.secrets.json
```

---

## Environment-Specific Configuration

### Development
- Use LocalDB or local SQL Server
- Connection string in `appsettings.json` or User Secrets

### Testing
- Use SQLite in-memory for integration tests
- Configured in test project, not API project

### Production (if deploying)
- Use Azure SQL Database or full SQL Server
- Connection string from environment variables or Key Vault

---

## Database Performance

### Indexes

Entity Framework automatically creates indexes for:
- Primary keys (clustered)
- Foreign keys

For production optimization, consider adding indexes on:
- `Tickets.Status` (for filtering)
- `Tickets.CreatedAt` (for sorting)

### Query Performance

Monitor queries with:
- SQL Server Profiler
- Entity Framework logging
- SSMS Query Execution Plans

---

## Backup and Restore (Optional)

### Creating Backup

```sql
BACKUP DATABASE TicketSystemDb
TO DISK = 'C:\Backups\TicketSystemDb.bak'
WITH FORMAT, INIT;
```

### Restoring Backup

```sql
RESTORE DATABASE TicketSystemDb
FROM DISK = 'C:\Backups\TicketSystemDb.bak'
WITH REPLACE;
```

---

## Additional Resources

- [EF Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [SQL Server LocalDB Documentation](https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb)
- [Connection Strings Reference](https://www.connectionstrings.com/sql-server/)

---

**Setup Documentation By:** [Your Name]  
**Date:** [Date]  
**Last Updated:** [Date]  
**Verified:** [YES/NO]
