# Cursor AI Instructions

## Purpose
This document provides persistent instructions for how Cursor AI should assist with this project throughout the development lifecycle.

---

## General Instructions

### Code Generation
1. **Always follow conventions** in `.cursorrules` file
2. **Use modern C# 12 patterns**: file-scoped namespaces, primary constructors where appropriate, required properties
3. **Async all the way**: Use async/await for all I/O operations
4. **Proper error handling**: Include try-catch blocks, validation, meaningful error messages
5. **Complete implementations**: Generate fully working code, not stubs or TODO comments

### Context Awareness
- Review `project-context.md` for project overview
- Check `spec.md` for requirements
- Refer to `tasks.md` for current phase and priorities
- Follow patterns established in existing code

### Code Quality
- Keep methods small (30-40 lines max)
- Single Responsibility Principle
- DRY (Don't Repeat Yourself)
- Meaningful variable and method names
- No obvious or redundant comments

---

## Phase-Specific Instructions

### Phase 4: Database Implementation
When generating database code:
- Use Fluent API for entity configurations, not data annotations
- Configure all relationships explicitly
- Include proper cascade delete rules
- Add indexes for foreign keys (automatic) and frequently queried fields
- Use UTC timestamps for DateTime fields

**Example DbContext Configuration:**
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Ticket>(entity =>
    {
        entity.HasKey(t => t.Id);
        
        entity.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);
            
        entity.HasOne(t => t.CreatedBy)
            .WithMany()
            .HasForeignKey(t => t.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);
            
        // More configurations...
    });
}
```

### Phase 5: Backend Implementation
When generating API code:

#### Controllers
- Return `ActionResult<T>` from action methods
- Use proper HTTP status codes (200, 201, 204, 400, 404, 500)
- Handle null cases and return 404 appropriately
- Include XML comments on public methods (optional but nice)

#### Services
- Inject dependencies via constructor
- Use interfaces for all services
- Return DTOs, not entities
- Validate business rules before database operations
- Call `SaveChangesAsync()` after modifications

#### State Machine (CRITICAL)
For the TicketStateMachine:
- Use switch expressions or statements
- Cover ALL possible status combinations
- Throw custom exception for invalid transitions
- Include clear error messages explaining why transition is invalid
- Test every single transition

**Example State Machine:**
```csharp
public bool CanTransition(TicketStatus from, TicketStatus to)
{
    return from switch
    {
        TicketStatus.Open => to is TicketStatus.InProgress or TicketStatus.Cancelled,
        TicketStatus.InProgress => to is TicketStatus.Resolved or TicketStatus.Cancelled,
        TicketStatus.Resolved => to == TicketStatus.Closed,
        TicketStatus.Closed => false,
        TicketStatus.Cancelled => false,
        _ => false
    };
}
```

### Phase 6: Frontend Implementation
When generating Blazor code:

#### Pages
- Use `@page` directive for routing
- Inject services with `@inject`
- Include loading state boolean
- Include error message string (nullable)
- Handle errors in try-catch with user-friendly messages
- Call `StateHasChanged()` after async updates if needed

#### Components
- Use `[Parameter]` for component parameters
- Emit events with `EventCallback<T>`
- Keep components focused and reusable
- Use Bootstrap 5 classes for styling

**Example Page Structure:**
```csharp
@page "/tickets"
@inject ITicketApiService TicketService

<h3>Tickets</h3>

@if (isLoading)
{
    <div class="spinner-border" role="status">
        <span class="visually-hidden">Loading...</span>
    </div>
}
else if (errorMessage != null)
{
    <div class="alert alert-danger">@errorMessage</div>
}
else
{
    <!-- Content here -->
}

@code {
    private List<TicketDto> tickets = new();
    private bool isLoading = true;
    private string? errorMessage;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            tickets = await TicketService.GetAllTicketsAsync();
        }
        catch (Exception ex)
        {
            errorMessage = $"Error loading tickets: {ex.Message}";
        }
        finally
        {
            isLoading = false;
        }
    }
}
```

### Phase 7: Testing
When generating tests:
- Use descriptive test names: `MethodName_Scenario_ExpectedResult`
- Arrange-Act-Assert pattern
- Each test should be independent
- Use realistic test data
- Include both success and failure cases

**For State Machine Tests:**
- Create one test per transition (valid and invalid)
- Use `[Fact]` for each test
- Assert both the HTTP status code and the response content
- Verify database state after integration tests

---

## Prompting Best Practices

### When I Ask for Code Generation
1. Ask clarifying questions if requirements are unclear
2. Generate complete, working code
3. Include error handling
4. Follow project conventions
5. Add brief comments for complex logic only

### When I Ask for Review
1. Check for bugs and logic errors
2. Verify state machine rules if applicable
3. Look for security issues
4. Check best practices adherence
5. Suggest improvements, don't just validate

### When I Ask for Debugging Help
1. Analyze error messages carefully
2. Consider the context (which phase, which component)
3. Suggest specific debugging steps
4. Propose fixes with explanation
5. Consider edge cases that might cause the issue

### When I Ask for Tests
1. Cover happy path first
2. Then cover error cases
3. Include edge cases
4. Ensure tests are independent
5. Use meaningful test data

---

## What to Avoid

### Don't Generate
❌ Placeholder methods with "// TODO" comments  
❌ Hardcoded connection strings or secrets  
❌ Console.WriteLine for debugging in production code  
❌ Commented-out code  
❌ Overly complex methods (keep it simple)  
❌ Duplicate code (use DRY principle)

### Don't Assume
❌ Don't assume I want a specific pattern unless specified  
❌ Don't assume authentication is needed (users are seeded)  
❌ Don't introduce new libraries without asking  
❌ Don't skip error handling  
❌ Don't ignore the .cursorrules conventions

---

## Response Format Preferences

### Code Blocks
- Use C# syntax highlighting
- Include file name as comment if applicable
- Keep code examples complete (not fragments)
- Show relevant using statements

### Explanations
- Be concise but clear
- Explain "why" not just "what"
- Highlight important considerations
- Mention trade-offs when applicable

### Suggestions
- Offer alternatives when appropriate
- Explain pros and cons
- Let me make the final decision
- Be honest about limitations

---

## State Machine Validation Checklist

Since the state machine is the critical piece, always check:
1. ✓ All 5 valid transitions are allowed
2. ✓ All invalid transitions are rejected
3. ✓ Terminal states (Closed, Cancelled) cannot transition
4. ✓ Clear error messages for invalid transitions
5. ✓ Integration tests cover all transitions
6. ✓ UI only shows valid transition buttons

---

## Continuous Reminders

### During Implementation
- **Test as you go** - Don't wait until the end
- **Validate business rules** - Especially state machine
- **Handle errors gracefully** - User-friendly messages
- **Keep methods focused** - One responsibility per method
- **Use dependency injection** - Don't create dependencies manually

### During Testing
- **Test state machine exhaustively** - This is mandatory
- **Use realistic test data** - Not just "test1", "test2"
- **Clean up after tests** - Independent, isolated tests
- **Test error cases too** - Not just happy path

### During Review
- **Question everything** - Even if it works
- **Look for security issues** - SQL injection, XSS, etc.
- **Check for best practices** - Not just correctness
- **Consider maintainability** - Can someone else understand this?

---

## Success Indicators

You're helping effectively when:
✓ Generated code compiles without errors  
✓ Code follows project conventions  
✓ State machine logic is correct  
✓ Tests are comprehensive  
✓ Error handling is present  
✓ Code is maintainable  
✓ Solutions are explained, not just provided

---

**Last Updated:** [Date]  
**Document Owner:** [Your Name]
