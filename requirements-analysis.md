# Requirements Analysis

## Selected Project Option

Backend-Heavy - Support Ticket Management System

## My Understanding

[Describe in your own words what you understand the project requirements to be. This should demonstrate your comprehension of the business context and technical requirements.]

## Functional Requirements

### Core Features
1. **Ticket Management**
   - Create new tickets with title, description, priority
   - View list of all tickets
   - View individual ticket details
   - Update ticket fields (title, description, priority, assignee)
   - Change ticket status through state machine transitions
   - Search tickets by keyword
   - Filter tickets by status

2. **Comment System**
   - Add comments to tickets
   - View all comments for a ticket
   - Display comment author and timestamp

3. **User Management**
   - Seed users in database (Admin, Agent, User roles)
   - Assign tickets to users
   - Track ticket creator

4. **State Machine (Signature Piece)**
   - Enforce valid status transitions:
     - Open → In Progress
     - Open → Cancelled
     - In Progress → Resolved
     - In Progress → Cancelled
     - Resolved → Closed
   - Reject invalid transitions with appropriate error messages

## Non-Functional Requirements

### Data Persistence
- All data must persist across application restarts
- Use SQL Server with Entity Framework Core
- Proper database migrations

### Validation
- Server-side input validation for all operations
- Required field validation
- Data type and length constraints
- State machine transition validation

### Error Handling
- Meaningful error messages for validation failures
- Appropriate HTTP status codes (400, 404, 500)
- User-friendly error display in UI
- Global exception handling middleware

### Performance
- Responsive UI with loading indicators
- Efficient database queries
- Proper indexing on foreign keys

### Security
- No secrets or credentials committed to repository
- SQL injection prevention through EF Core parameterization
- Input sanitization

### Testing
- Integration tests for state machine transitions (mandatory)
- Unit tests for business logic
- Test coverage for edge cases

## Assumptions

[List your assumptions about requirements, user behavior, technical constraints, etc. For example:
- Users are already authenticated (no auth required for Core)
- Single-tenant application
- English language only
- etc.]

## Clarifications

[Note any ambiguities you identified in the requirements and how you resolved them]

## Edge Cases

### State Machine Edge Cases
- Attempt to transition from a terminal state (Closed, Cancelled)
- Concurrent status updates to the same ticket
- Invalid status value provided

### Data Validation Edge Cases
- Empty or whitespace-only input
- Input exceeding maximum length
- Special characters in text fields
- Non-existent user ID for assignment
- Non-existent ticket ID for updates/comments

### Search and Filter Edge Cases
- Empty search keyword
- No matching results
- Combined search and filter
- Case sensitivity in search

### Database Edge Cases
- Database connection failure
- Constraint violations (foreign key, unique)
- Transaction rollback scenarios
