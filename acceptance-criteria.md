# Acceptance Criteria

## Core Acceptance Criteria

### 1. Ticket Creation
- [ ] User can navigate to create ticket page
- [ ] Form includes fields: title, description, priority, assignee (optional)
- [ ] Form validates required fields before submission
- [ ] New ticket is saved to database with status "Open"
- [ ] User is redirected to ticket detail or list after creation
- [ ] Success message is displayed

### 2. Ticket Listing
- [ ] User can view all tickets in a list/table format
- [ ] List displays: ticket ID, title, status, priority, assignee, created date
- [ ] Each ticket is clickable to view details
- [ ] List loads from database on page load
- [ ] Empty state is shown if no tickets exist

### 3. Ticket Detail View
- [ ] User can view complete ticket information
- [ ] Display includes: ID, title, description, status, priority, assignee, creator, timestamps
- [ ] All comments are displayed below ticket details
- [ ] Comment form is available to add new comments
- [ ] Status transition buttons are available based on current status
- [ ] Edit button navigates to edit page

### 4. Ticket Updates
- [ ] User can navigate to edit ticket page
- [ ] Form is pre-populated with current ticket data
- [ ] User can update: title, description, priority, assignee
- [ ] Changes are saved to database
- [ ] UpdatedAt timestamp is automatically updated
- [ ] Success message is displayed

### 5. Comment System
- [ ] User can add a comment to any ticket
- [ ] Comment form validates message is not empty
- [ ] Comment is saved with author and timestamp
- [ ] Comment appears in ticket detail view immediately
- [ ] Comments are displayed in chronological order

### 6. State Machine Transitions (Critical)
**Valid Transitions (Must Succeed):**
- [ ] Ticket in "Open" status can transition to "In Progress"
- [ ] Ticket in "Open" status can transition to "Cancelled"
- [ ] Ticket in "In Progress" status can transition to "Resolved"
- [ ] Ticket in "In Progress" status can transition to "Cancelled"
- [ ] Ticket in "Resolved" status can transition to "Closed"

**Invalid Transitions (Must Be Rejected):**
- [ ] Ticket in "Open" status cannot transition to "Resolved"
- [ ] Ticket in "Open" status cannot transition to "Closed"
- [ ] Ticket in "Resolved" status cannot transition to "In Progress"
- [ ] Ticket in "Resolved" status cannot transition to "Open"
- [ ] Ticket in "Closed" status cannot transition to any other status
- [ ] Ticket in "Cancelled" status cannot transition to any other status

**UI Behavior:**
- [ ] Only valid transition buttons are displayed/enabled
- [ ] Invalid transitions return 400 Bad Request from API
- [ ] Error message is displayed in UI for invalid transitions

### 7. Search Functionality
- [ ] Search box is available on ticket list page
- [ ] User can enter keyword and submit search
- [ ] Results show tickets where keyword matches title or description
- [ ] Search is case-insensitive
- [ ] Empty search returns all tickets

### 8. Filter by Status
- [ ] Status filter dropdown is available on ticket list page
- [ ] Dropdown includes all status values plus "All"
- [ ] Selecting a status filters the ticket list
- [ ] Filter can be combined with search
- [ ] "All" option shows all tickets

### 9. Data Persistence
- [ ] All created tickets persist in database
- [ ] All comments persist in database
- [ ] Status changes persist in database
- [ ] Ticket updates persist in database
- [ ] Data survives application restart (verified by manual test)

### 10. Security
- [ ] No connection strings with passwords committed to repository
- [ ] No API keys or secrets in code
- [ ] Connection strings use environment variables or user secrets
- [ ] .gitignore properly configured for .NET projects

### 11. Testing
- [ ] State machine integration tests exist and pass
- [ ] All valid transitions have passing tests
- [ ] All invalid transitions have failing tests that assert 400 error
- [ ] Additional integration tests for CRUD operations
- [ ] Test results documented

## Validation Acceptance Criteria

### Ticket Validation
- [ ] Title is required (non-empty, non-whitespace)
- [ ] Title max length is 200 characters
- [ ] Description is required
- [ ] Description max length is 5000 characters
- [ ] Priority must be one of: Low, Medium, High, Critical
- [ ] Status must be one of: Open, InProgress, Resolved, Closed, Cancelled
- [ ] AssignedToId must reference existing user (if provided)
- [ ] CreatedById must reference existing user

### Comment Validation
- [ ] Message is required (non-empty, non-whitespace)
- [ ] Message max length is 2000 characters
- [ ] TicketId must reference existing ticket
- [ ] CreatedById must reference existing user

### API Response Validation
- [ ] 400 Bad Request for validation failures
- [ ] 404 Not Found for non-existent resources
- [ ] 200 OK for successful GET requests
- [ ] 201 Created for successful POST requests
- [ ] 204 No Content for successful PUT/DELETE requests

## Error Handling Acceptance Criteria

### Backend Error Handling
- [ ] Global exception middleware catches unhandled exceptions
- [ ] Validation errors return structured error response
- [ ] State machine violations return clear error message
- [ ] Database errors are caught and logged
- [ ] Error responses include appropriate HTTP status codes

### Frontend Error Handling
- [ ] API errors are caught and displayed to user
- [ ] Loading states prevent duplicate submissions
- [ ] Form validation provides immediate feedback
- [ ] Network errors show user-friendly messages
- [ ] Success operations show confirmation messages

## Testing Acceptance Criteria

### Required Tests
- [ ] State machine integration tests for all transitions
- [ ] Ticket CRUD integration tests
- [ ] Comment creation integration tests
- [ ] Search and filter integration tests
- [ ] Validation error tests
- [ ] State machine unit tests
- [ ] Service layer unit tests (optional but recommended)

### Test Quality
- [ ] Tests use meaningful test data
- [ ] Tests clean up after themselves
- [ ] Tests are independent and can run in any order
- [ ] Test names clearly describe what is being tested
- [ ] Tests include arrange, act, assert sections

## Documentation Acceptance Criteria

### Code Documentation
- [ ] README.md with setup instructions
- [ ] Database setup documented
- [ ] API endpoints documented (api-contract.md)
- [ ] Data model documented (data-model.md)
- [ ] Public methods have XML comments (optional)

### Lifecycle Documentation
- [ ] tool-workflow.md completed (Part A)
- [ ] requirements-analysis.md completed
- [ ] acceptance-criteria.md completed (this file)
- [ ] implementation-plan.md completed
- [ ] design-notes.md completed
- [ ] test-strategy.md completed
- [ ] test-results.md completed
- [ ] debugging-notes.md completed
- [ ] code-review-notes.md completed
- [ ] reflection.md completed
- [ ] pr-description.md completed
- [ ] All ai-prompts/ files document prompt history with iterations
