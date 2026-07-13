# Design Notes

## Architecture Overview

### System Architecture

[Describe the overall system architecture: client-server model, separation of concerns, layering approach]

### Technology Decisions

**Frontend: Blazor WebAssembly**
- Rationale: [Why Blazor was chosen]
- Benefits: [C# full-stack, type safety, component reusability]
- Trade-offs: [Bundle size, initial load time]

**Backend: ASP.NET Core Web API**
- Rationale: [Why .NET 9 Web API was chosen]
- Benefits: [Performance, ecosystem, tooling]
- Architecture pattern: [Controller → Service → Repository/DbContext]

**Database: SQL Server with Entity Framework Core**
- Rationale: [Why SQL Server and EF Core]
- Benefits: [Relational data model fits requirements, strong typing, migrations]
- Trade-offs: [Setup complexity vs. simplicity of in-memory DBs]

## Frontend Design

### Component Structure

[Describe your Blazor component hierarchy and organization]

**Pages:**
- TicketList
- TicketDetail
- CreateTicket
- EditTicket

**Shared Components:**
- StatusBadge
- PriorityBadge
- CommentList
- AddComment
- SearchFilter

### State Management

[How you manage state in the Blazor app: component parameters, cascading parameters, local state, service injection]

### API Communication

[How the frontend communicates with the backend: HttpClient, service classes, error handling, loading states]

### UI/UX Patterns

[Design patterns used: forms, validation feedback, loading indicators, error messages, confirmation dialogs]

## Backend Design

### Layering Architecture

```
Controllers (HTTP endpoints)
    ↓
Services (Business logic)
    ↓
State Machine (Validation)
    ↓
DbContext/Repositories (Data access)
    ↓
Database
```

### Controller Layer

[Responsibilities: HTTP request/response, model binding, routing, delegating to services]

### Service Layer

[Responsibilities: Business logic, orchestration, validation coordination, transaction management]

### State Machine Design

[How the state machine is implemented, interface design, validation logic, extensibility]

**Interface:**
```csharp
public interface ITicketStateMachine
{
    bool CanTransition(TicketStatus from, TicketStatus to);
    void ValidateTransition(TicketStatus from, TicketStatus to);
}
```

**Valid Transitions:**
- Open → InProgress, Cancelled
- InProgress → Resolved, Cancelled
- Resolved → Closed

[Describe how you enforce these rules]

### Data Access Layer

[How you use EF Core: DbContext, DbSet, Fluent API configurations, relationships]

## Database Design

### Entity Relationships

```
User 1 --→ * Ticket (CreatedBy)
User 1 --→ * Ticket (AssignedTo)
User 1 --→ * Comment (CreatedBy)
Ticket 1 --→ * Comment
```

### Entities

**User**
- Seeded data only, no CRUD operations
- Roles: Admin, Agent, User

**Ticket**
- Core entity with CRUD operations
- State machine-enforced status field
- Audit fields: CreatedAt, UpdatedAt

**Comment**
- Child of Ticket
- Immutable once created
- Audit field: CreatedAt

### Indexes and Constraints

[Describe any indexes you added beyond foreign keys, unique constraints, check constraints]

### Migration Strategy

[How you manage database migrations, versioning, seed data]

## Validation Strategy

### Server-Side Validation

**Approach:** [FluentValidation or Data Annotations]

**Validation Rules:**
- Required fields
- String length limits
- Enum value validation
- Foreign key existence checks
- State machine transition validation

### Client-Side Validation

[How validation feedback is provided in the UI]

## Error Handling Strategy

### Backend Error Handling

**Global Exception Middleware:**
[How you catch and handle unhandled exceptions]

**Validation Errors:**
[How you return 400 Bad Request with validation details]

**Not Found Errors:**
[How you return 404 for missing resources]

**State Machine Violations:**
[How you return 400 with clear error message for invalid transitions]

### Frontend Error Handling

[How you display errors to users: toast notifications, inline messages, error pages]

## Testing Strategy

[Link to test-strategy.md]

### Test Structure
- Unit tests for state machine logic
- Integration tests for API endpoints
- State machine transition tests (mandatory)

### Test Data
- In-memory database for tests
- Seed data in test setup
- Cleanup in test teardown

## Security Considerations

- No secrets in source code
- Environment variables for connection strings
- SQL injection prevention via EF Core
- Input validation and sanitization
- CORS configuration for Blazor frontend

## Performance Considerations

- Efficient queries with EF Core
- Eager loading for related data where appropriate
- Pagination (if implemented as stretch)
- Async/await throughout

## Scalability Considerations

[How the design could scale: caching, load balancing, database optimization]

## Future Enhancements

[Ideas for Stretch features or future improvements:
- Authentication and authorization
- Real-time updates with SignalR
- Email notifications
- File attachments
- Ticket history/audit log
- Advanced reporting]
