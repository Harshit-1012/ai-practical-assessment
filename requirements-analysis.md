# Requirements Analysis

## Selected Project Option

Backend-Heavy - Support Ticket Management System

## My Understanding

I am building a Support Ticket Management System that allows internal users to create and manage support tickets. Users can raise new tickets, view their status, update them, and close them when the issue is resolved.Each ticket follows a fixed workflow: Open → In Progress → Resolved → Closed, with an option to Cancel before it is completed. These status changes are validated on the backend to ensure users cannot skip or make invalid transitions.Users can also add comments to tickets to keep track of updates and discussions. The system provides search and filter options to quickly find tickets based on keywords or status.The application stores all data in SQL Server using Entity Framework Core and includes proper validation and error handling so that invalid requests or status changes are handled gracefully and meaningful error messages are shown to users.

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
- If a ticket doesn't have an assigned user, it is simply considered unassigned—it's not treated as an error.
- Once a ticket is marked as Cancelled or Closed, its status cannot be changed anymore.
- The system comes with a predefined list of users. There is no user registration or login functionality in this version.
- Comments can be added to a ticket, but they cannot be edited or deleted after they are posted.
- Priority and Status have predefined values (enums) and users cannot create or modify these options.


## Clarifications

- The requirements don't say what should happen if a ticket is assigned to a user ID that doesn't exist. I'm assuming the request should be rejected with a 400 Bad Request validation error.
- The requirements don't mention whether empty or whitespace-only comments are allowed. I'm assuming they should be rejected because they don't provide any useful information.

- The requirements don't explain what should happen to a ticket's comments if the ticket is deleted. Since deleting tickets isn't part of the core features, I'm treating this as out of scope for now.

## Edge Cases
- What should happen if two users try to update the same ticket's status at the same time (race condition)? The system should handle this situation properly    to avoid conflicts?
- Should the system reject comments that are empty or contain only spaces?
- What should happen if a ticket is assigned to a user ID that doesn't exist in the system?
- If a user searches with an empty keyword, should the system return all tickets instead of no results?
- If a ticket is deleted, what should happen to the comments linked to it? (Even though deleting tickets isn't part of the core features.)

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
