# Test Strategy

## Test Scope

### In Scope for Core
- State machine transition logic (mandatory)
- Ticket CRUD operations via API
- Comment creation via API
- Search and filter functionality
- Input validation
- Error handling (validation errors, not found, invalid transitions)
- Data persistence

### Out of Scope for Core (Stretch)
- UI component testing (Blazor components)
- End-to-end browser tests
- Performance and load testing
- Security penetration testing
- Concurrency and race condition testing

---

## Test Tiers

### Unit Tests

**Purpose:** Test individual components in isolation

**Coverage:**
- State machine logic (`TicketStateMachine`)
  - Test `CanTransition()` for all valid combinations
  - Test `CanTransition()` returns false for invalid combinations
  - Test `ValidateTransition()` throws exception for invalid transitions
- Service layer methods (if testable without database)
- Validators (if using FluentValidation)

**Tools:**
- xUnit
- Moq (for mocking dependencies)

**Example Tests:**
```csharp
[Fact]
public void CanTransition_OpenToInProgress_ReturnsTrue()

[Fact]
public void CanTransition_OpenToResolved_ReturnsFalse()

[Fact]
public void ValidateTransition_OpenToClosed_ThrowsException()
```

---

### API Integration Tests (Mandatory)

**Purpose:** Test API endpoints with actual database, verifying end-to-end behavior

**Coverage:**

**1. State Machine Integration Tests (MANDATORY - Signature Piece)**
- Test all 5 valid transitions succeed
  - Open → InProgress
  - Open → Cancelled
  - InProgress → Resolved
  - InProgress → Cancelled
  - Resolved → Closed
- Test invalid transitions return 400 Bad Request
  - Open → Resolved
  - Open → Closed
  - Resolved → InProgress
  - Closed → any status
  - Cancelled → any status
- Verify status persists in database after transition

**2. Ticket CRUD Tests**
- Create ticket: POST /api/tickets
  - Valid ticket creation succeeds with 201
  - Ticket is saved to database
  - Status is automatically set to "Open"
- Get ticket by ID: GET /api/tickets/{id}
  - Existing ticket returns 200 with data
  - Non-existent ticket returns 404
- Get all tickets: GET /api/tickets
  - Returns all tickets from database
  - Empty database returns empty array
- Update ticket: PUT /api/tickets/{id}
  - Valid update succeeds with 200
  - UpdatedAt timestamp is modified
  - Changes persist in database
- Update non-existent ticket returns 404

**3. Comment Tests**
- Create comment: POST /api/tickets/{ticketId}/comments
  - Valid comment creation succeeds with 201
  - Comment is saved to database with timestamp
  - Creating comment on non-existent ticket returns 404
- Get comments: GET /api/tickets/{ticketId}/comments
  - Returns all comments for ticket
  - Comments are in chronological order

**4. Search and Filter Tests**
- Search by keyword: GET /api/tickets?keyword=X
  - Returns tickets matching keyword in title or description
  - Search is case-insensitive
  - Empty keyword returns all tickets
- Filter by status: GET /api/tickets?status=Open
  - Returns only tickets with matching status
  - Invalid status returns appropriate error
- Combined search and filter
  - Both parameters work together

**5. Validation Tests**
- Create ticket with missing title returns 400
- Create ticket with empty description returns 400
- Create ticket with invalid priority returns 400
- Create ticket with non-existent assignedToId returns 400
- Update ticket with title exceeding max length returns 400
- Create comment with empty message returns 400

**Tools:**
- xUnit
- WebApplicationFactory (for in-memory API hosting)
- SQLite in-memory database (for fast, isolated tests)
- FluentAssertions (optional, for readable assertions)

---

## Test Structure

### Test Project Organization
```
tests/TicketSystem.Tests/
├── UnitTests/
│   ├── TicketStateMachineTests.cs
│   └── ...
├── IntegrationTests/
│   ├── TicketsControllerTests.cs
│   ├── CommentsControllerTests.cs
│   └── StateTransitionIntegrationTests.cs (CRITICAL)
└── Helpers/
    ├── TestWebApplicationFactory.cs
    └── TestDataSeeder.cs
```

### Test Naming Convention

```csharp
[Fact]
public void MethodName_Scenario_ExpectedResult()

// Examples:
[Fact]
public void CreateTicket_ValidData_Returns201Created()

[Fact]
public void ChangeStatus_InvalidTransition_Returns400BadRequest()

[Fact]
public void GetTicket_NonExistentId_Returns404NotFound()
```

---

## Test Data Management

### Seed Data Approach
- Each test class sets up its own test data in constructor or setup method
- Use a separate in-memory database instance per test class
- Clean database between tests (or use new database instance)

### Sample Test Data
```csharp
// Users
User adminUser = new() { Id = 1, Name = "Test Admin", Email = "admin@test.com", Role = "Admin" };
User agentUser = new() { Id = 2, Name = "Test Agent", Email = "agent@test.com", Role = "Agent" };
User normalUser = new() { Id = 3, Name = "Test User", Email = "user@test.com", Role = "User" };

// Tickets
Ticket openTicket = new() 
{ 
    Id = 1, 
    Title = "Test Ticket", 
    Description = "Test Description",
    Status = "Open",
    Priority = "Medium",
    CreatedById = 3,
    CreatedAt = DateTime.UtcNow,
    UpdatedAt = DateTime.UtcNow
};
```

---

## Test Execution

### Running All Tests
```bash
cd tests/TicketSystem.Tests
dotnet test
```

### Running Specific Test Category
```bash
dotnet test --filter "Category=Integration"
dotnet test --filter "Category=StateMachine"
```

### Generating Coverage Report (Optional)
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

---

## Edge Case Tests

### State Machine Edge Cases
- Attempt to transition from terminal state (Closed, Cancelled)
- Provide invalid status string
- Null or empty status value
- Transition to same status (Open → Open)

### Data Validation Edge Cases
- Whitespace-only strings for required fields
- Maximum length boundary testing
- Minimum length boundary testing
- Special characters in text fields
- Unicode characters

### Database Edge Cases
- Foreign key constraint violations
- Unique constraint violations (if any)
- Concurrent updates to same ticket
- Database connection failure (optional)

---

## Test Coverage Goals

### Minimum Coverage (Core)
- **State Machine:** 100% of transition logic
- **Controllers:** All API endpoints
- **Critical Paths:** Create, Update, Status Change
- **Validation:** All validation rules

### Stretch Coverage
- All service methods
- Edge cases and error paths
- Concurrent operations
- Performance benchmarks

---

## Tests Not Covered (Acknowledged Limitations)

### Out of Scope for This Exercise
- **UI Tests:** Blazor component rendering, user interactions
- **End-to-End Tests:** Full user workflows through browser
- **Load Tests:** Performance under high concurrency
- **Security Tests:** SQL injection, XSS, CSRF
- **Integration with External Systems:** None in this project
- **Manual Testing:** Will be performed but not automated

### Rationale
- Limited time (1 week for full project + documentation)
- Core acceptance criteria focus on API and state machine
- UI testing requires additional tooling (bUnit, Playwright)
- Manual testing will verify UI functionality

---

## Test Results Documentation

All test execution results will be documented in `test-results.md` including:
- Test run date and time
- Number of tests passed/failed
- Any failing tests and root causes
- Screenshots of test output
- Coverage metrics (if collected)

---

## Continuous Testing During Development

### Test-Driven Development Approach (Optional)
1. Write failing test for new feature
2. Implement minimum code to make test pass
3. Refactor and ensure tests still pass

### Regular Test Execution
- Run tests after each significant code change
- Run full test suite before committing
- Fix failing tests immediately
- Do not commit broken tests

### State Machine Testing Priority
- State machine tests are written first
- All state machine tests must pass before moving forward
- State machine is the "signature piece" - highest quality bar
