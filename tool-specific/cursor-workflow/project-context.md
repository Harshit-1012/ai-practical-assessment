# Project Context - Support Ticket Management System

## Project Overview

**Project Name:** Support Ticket Management System  
**Purpose:** .NET AI Capability Exercise - Demonstrating effective AI-assisted full-stack development  
**Timeline:** 1 week (self-paced)  
**Primary Goal:** Build a working ticket management system while documenting AI usage across the entire software lifecycle

## Business Context

This is an internal support ticket tracking application where users can:
- Create support tickets for issues
- Assign tickets to support agents
- Progress tickets through a defined lifecycle (state machine)
- Add comments for collaboration
- Search and filter tickets

## Technical Stack

### Frontend
- **Framework:** Blazor WebAssembly
- **UI Library:** Bootstrap 5
- **Language:** C# 12
- **Runtime:** .NET 9

### Backend
- **Framework:** ASP.NET Core Web API
- **Language:** C# 12
- **Runtime:** .NET 9
- **Architecture:** Controller → Service → DbContext layers

### Database
- **Database:** SQL Server (LocalDB for development)
- **ORM:** Entity Framework Core 9.0
- **Migrations:** EF Core Migrations

### Testing
- **Framework:** xUnit
- **Integration Tests:** WebApplicationFactory with SQLite in-memory
- **Test Database:** SQLite (for tests), SQL Server (for app)

## Project Structure

```
ai-practical-assessment/
├── src/
│   ├── TicketSystem.Api/         # Backend API
│   └── TicketSystem.Blazor/      # Frontend app
├── tests/
│   └── TicketSystem.Tests/       # Unit & integration tests
├── database/
│   ├── schema-or-migrations/     # EF Core migrations
│   ├── seed-data/                # Sample data scripts
│   └── setup-notes.md
├── ai-prompts/                   # AI prompt history
│   ├── planning.md
│   ├── design.md
│   ├── implementation.md
│   ├── testing.md
│   ├── debugging.md
│   ├── code-review.md
│   └── documentation.md
├── tool-specific/cursor-workflow/ # Cursor AI context
│   ├── project-context.md        # This file
│   ├── spec.md                   # Requirements spec
│   ├── tasks.md                  # Task breakdown
│   └── cursor-rules-or-instructions.md
├── [Various lifecycle docs]
└── README.md
```

## Core Features (Mandatory)

### 1. Ticket Management
- Create tickets with title, description, priority, assignee
- View all tickets in a list
- View individual ticket details
- Update ticket fields (not status - see state machine)
- Persist all data in SQL Server

### 2. State Machine (SIGNATURE PIECE)
**This is the critical engineering judgment piece**

**Valid Transitions:**
```
Open → InProgress
Open → Cancelled
InProgress → Resolved
InProgress → Cancelled
Resolved → Closed
```

**Implementation Requirements:**
- Backend must enforce these rules strictly
- Invalid transitions return 400 Bad Request
- Frontend should only show valid transition buttons
- Comprehensive integration tests for all transitions
- This is what demonstrates engineering capability

### 3. Comment System
- Add comments to any ticket
- View all comments for a ticket
- Comments include author and timestamp
- Comments are immutable once created

### 4. Search and Filter
- Search tickets by keyword (title or description)
- Filter tickets by status
- Both can be combined
- Case-insensitive search

### 5. Data Persistence
- All data stored in SQL Server
- Data survives application restart (must be verified)
- Database migrations for schema management
- Seed data for users

## Stretch Features (Optional)

Only implement if Core is complete and time permits:
- User authentication (JWT or session)
- Role-based access control
- Advanced filtering (priority, assignee, date range)
- Pagination
- Sorting
- Real-time updates (SignalR)
- Additional test coverage
- Docker containerization
- CI/CD pipeline

## Key Constraints

### Time Constraints
- One week total for project + documentation
- Core features prioritized over Stretch
- Documentation is 60% of evaluation (Part B)
- State machine must be perfect

### Technical Constraints
- Must use .NET 9 (latest)
- Must use SQL Server (not SQLite or in-memory for app)
- Must use Entity Framework Core
- Must include integration tests for state machine
- No secrets committed to repository

### Documentation Requirements
- Complete prompt history in ai-prompts/
- All lifecycle documents required
- Show iteration and validation, not copy-paste
- Demonstrate AI workflow maturity

## Entities

### User (Seeded Only)
```csharp
public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // Admin, Agent, User
}
```

### Ticket
```csharp
public class Ticket
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty; // Low, Medium, High, Critical
    public string Status { get; set; } = string.Empty; // Open, InProgress, Resolved, Closed, Cancelled
    public int? AssignedToId { get; set; }
    public User? AssignedTo { get; set; }
    public int CreatedById { get; set; }
    public User CreatedBy { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<Comment> Comments { get; set; } = new();
}
```

### Comment
```csharp
public class Comment
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public Ticket Ticket { get; set; } = null!;
    public string Message { get; set; } = string.Empty;
    public int CreatedById { get; set; }
    public User CreatedBy { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
```

## Success Criteria

### Technical Completeness
- All Core features working
- State machine enforces all rules
- Data persists across restarts (verified)
- Tests pass (especially state machine tests)
- No secrets in repository
- Application runs locally

### Documentation Completeness (Critical)
- tool-workflow.md (Part A) complete
- All planning documents complete
- Complete prompt history showing iteration
- Test strategy and results documented
- Debugging notes with AI usage
- Code review notes
- Reflection with honest assessment
- PR description ready

### Quality Indicators
- Clean, readable code
- Proper error handling
- User-friendly UI messages
- Comprehensive state machine tests
- Validated AI output (not blind copy-paste)

## Non-Goals

### What This Project Is NOT
- Not a production-ready system
- Not focused on scalability at this stage
- Not implementing advanced features like notifications
- Not a UI/UX showcase (Bootstrap defaults are fine)
- Not testing Blazor components (API tests are priority)

### What Can Be Simplified
- No complex authentication (users are seeded)
- No authorization checks (all users can do everything)
- No audit logging (beyond timestamps)
- No file attachments
- No email notifications
- No sophisticated caching

## Risk Areas

### High Risk
1. **State Machine Logic** - Most complex part, needs perfect implementation
2. **Time Management** - Documentation takes longer than expected
3. **AI Validation** - Temptation to accept AI output without testing

### Medium Risk
1. **Database Setup** - SQL Server configuration can have issues
2. **CORS Configuration** - Blazor-API communication
3. **Prompt Documentation** - Easy to forget to document during active coding

### Low Risk
1. **Basic CRUD** - Straightforward with EF Core
2. **UI Layout** - Bootstrap templates available
3. **Test Framework** - xUnit is well-documented

## AI Tool Usage

### Primary Tool
**Cursor AI** - Integrated into development environment

### How AI Will Be Used
1. **Requirements Analysis** - Extract and organize requirements
2. **Design** - Generate diagrams, API specs, data models
3. **Implementation** - Code generation, boilerplate, patterns
4. **Testing** - Test case generation, test data
5. **Debugging** - Error analysis, fix suggestions
6. **Code Review** - Quality, security, best practices
7. **Documentation** - README, API docs, comments

### Context Provision
- .cursorrules file with project standards
- This project-context.md file
- spec.md with requirements
- tasks.md with task breakdown
- Inline comments and file context

### Validation Strategy
- Read all AI-generated code
- Test every generated function
- Verify against requirements
- Check for security issues
- Document what was changed and why

## Target Audience for This Project

### Primary Evaluator
Someone assessing AI workflow maturity across the lifecycle

### What They're Looking For
- Clear requirements understanding
- Effective AI prompting with context
- Iteration and refinement, not copy-paste
- Validation of AI output
- Honest reflection on what worked and what didn't
- Reusable workflow patterns
- Both working code AND comprehensive documentation

## Project Phases

See `implementation-plan.md` for detailed phase breakdown:
1. Foundation & Setup
2. Cursor Context & AI Workflow (Part A)
3. Requirement Analysis & Planning
4. Database Design & Implementation
5. Backend API Implementation
6. Frontend Implementation
7. Testing Implementation
8. Testing & Debugging
9. Code Review & Refinement
10. Documentation & Finalization
11. Stretch Features (optional)

## Current Status

**Phase:** [Will be updated as project progresses]  
**Completed:** [List completed phases]  
**In Progress:** [Current phase]  
**Next:** [Next phase]

---

**Document Owner:** [Your Name]  
**Last Updated:** [Date]  
**Project Start Date:** [Date]
