# Implementation Plan

## Overview

Build a Support Ticket Management System demonstrating AI-assisted full-stack development with comprehensive lifecycle documentation. The system uses Blazor WebAssembly for the frontend, ASP.NET Core Web API (.NET 9) for the backend, and SQL Server with Entity Framework Core for data persistence.

**Core Focus:** A backend-heavy application featuring a state-machine-enforced ticket lifecycle, search/filter capabilities, and full CRUD operations with comments. The state machine validation is the signature engineering piece requiring rigorous testing.

**Evaluation Structure:** Per spec, Part A (AI Workflow Foundation) is 20%, Part B (Full-Stack Project including Core features + lifecycle artifacts) is 60%, and Part C (Submission and Reflection) is 20%. Both the working application and the comprehensive documentation are essential components of Part B.

**Technology Stack:**
- Frontend: Blazor WebAssembly
- Backend: ASP.NET Core Web API (.NET 9)
- Database: SQL Server with EF Core
- Testing: xUnit with integration tests
- Location: d:\ai-practical-assessment

## Task Breakdown

### Phase 1: Foundation & Setup
**Goal:** Establish project structure, version control, and complete folder hierarchy.

1. Create project directory structure
2. Initialize Git repository with .gitignore
3. Create .NET solution with three projects:
   - `TicketSystem.Api` (ASP.NET Core Web API)
   - `TicketSystem.Blazor` (Blazor WebAssembly)
   - `TicketSystem.Tests` (xUnit)
4. Create all documentation file templates (headers/sections only)

**Deliverables:** Complete repository structure, project solution files, documentation templates

### Phase 2: Cursor Context & AI Workflow Foundation (Part A)
**Goal:** Configure persistent AI context before implementation begins.

1. Create `.cursorrules` file with .NET 9/C# 12 conventions, Blazor patterns, EF Core practices, state machine requirements
2. Create `tool-specific/cursor-workflow/` files:
   - `project-context.md` - project overview and constraints
   - `spec.md` - copy of this specification
   - `tasks.md` - prioritized task breakdown
   - `cursor-rules-or-instructions.md` - persistent Cursor instructions
3. Write `tool-workflow.md` (Part A) documenting:
   - Primary tool (Cursor AI)
   - Context provision approach
   - AI usage across lifecycle phases
   - Information boundaries (no secrets)
   - Reusable workflow patterns

**Deliverables:** Cursor configuration, tool-workflow.md, reusable context files

### Phase 3: Requirement Analysis & Planning Artifacts
**Goal:** Create structured planning documents (templates with headers).

1. `candidate-info.md` - personal details and project summary
2. `requirements-analysis.md` - functional/non-functional requirements, assumptions, edge cases
3. `acceptance-criteria.md` - Core criteria, validation, error handling, testing, documentation
4. `design-notes.md` - architecture, frontend/backend/database design, validation/error handling/testing strategies
5. `api-contract.md` - endpoint specifications, request/response formats, validation rules
6. `data-model.md` - entity definitions, relationships, state machine rules
7. `ui-flow.md` - user journey documentation
8. `test-strategy.md` - test scope, tiers, edge cases

**Deliverables:** Complete planning artifact templates (User fills Understanding/Assumptions sections)

### Phase 4: Database Design & Implementation
**Goal:** Implement database layer with verified persistence.

1. Design schema: Users, Tickets, Comments entities
2. Create EF Core `AppDbContext` with Fluent API configurations
3. Create initial migration
4. Implement seed data (3-5 sample users)
5. Configure connection string in appsettings.json
6. **Critical:** Verify data persistence (create ticket, restart API, confirm ticket exists)
7. Document setup in `database/setup-notes.md`

**Deliverables:** Working database, migrations, seed data, setup documentation, persistence verification

### Phase 5: Backend API Implementation
**Goal:** Build RESTful API with state machine enforcement.

#### 5.1: Core Infrastructure
1. Create entities: User, Ticket, Comment
2. Create DTOs for requests/responses
3. Implement global exception handling middleware
4. Configure CORS for Blazor frontend

#### 5.2: State Machine (CRITICAL PIECE)
1. Design `ITicketStateMachine` interface
2. Implement `TicketStateMachine` class with transition validation:
   - Open → InProgress, Cancelled
   - InProgress → Resolved, Cancelled
   - Resolved → Closed
3. Create unit tests for all valid and invalid transitions

#### 5.3: Services Layer
1. Implement `TicketService` with CRUD operations
2. Integrate state machine validation in status updates
3. Implement `CommentService` for comment operations
4. Add input validation using FluentValidation or Data Annotations

#### 5.4: Controllers
1. `TicketsController`: List, Get, Create, Update, ChangeStatus, Search
2. `CommentsController`: Create, List by ticket
3. `UsersController`: List (read-only for dropdowns)

**Deliverables:** Complete API with state machine, services, controllers, validation, error handling

### Phase 6: Frontend Implementation
**Goal:** Build Blazor WebAssembly UI with full ticket management.

1. Create HTTP API service layer (`TicketApiService`, `CommentApiService`)
2. Implement pages:
   - `TicketList.razor` - display all tickets, search box, status filter
   - `TicketDetail.razor` - ticket info, comments, status change buttons, edit link
   - `CreateTicket.razor` - creation form with validation
   - `EditTicket.razor` - update form
3. Create reusable components:
   - `StatusBadge`, `PriorityBadge` - visual indicators
   - `CommentList`, `AddComment` - comment functionality
   - `SearchFilter` - search and filter controls
4. Implement status transition logic (only show valid transition buttons)
5. Add loading states, error handling, success notifications
6. Style with Bootstrap 5 (responsive design)

**Deliverables:** Complete Blazor frontend with all Core features

### Phase 7: Testing Implementation
**Goal:** Write mandatory state machine integration tests and additional test coverage.

1. **State Machine Integration Tests (MANDATORY):**
   - Test all 5 valid transitions succeed
   - Test all invalid transitions return 400 error
   - Verify persistence of status changes
2. Additional integration tests:
   - Ticket CRUD operations
   - Comment creation
   - Search and filter functionality
   - Validation error responses
3. Unit tests:
   - State machine logic
   - Service layer methods
   - Input validators
4. Setup WebApplicationFactory for API testing
5. Use in-memory SQLite for test database
6. Document test strategy and results

**Deliverables:** Passing test suite, test-results.md, test-strategy.md

### Phase 8: Testing & Debugging
**Goal:** Verify functionality and fix issues with AI assistance.

1. Run complete test suite, capture results in `test-results.md`
2. Fix failing tests
3. Manual testing through Blazor UI
4. Test state machine transitions manually
5. Verify edge cases (boundary values, invalid inputs)
6. Document issues in `debugging-notes.md`:
   - Problem description
   - Investigation approach
   - How AI assisted diagnosis
   - Manual validation performed
   - Final fix applied

**Deliverables:** All tests passing, debugging documentation, stable application

### Phase 9: Code Review & Refinement
**Goal:** AI-assisted code review with critical evaluation.

1. Use AI for comprehensive code review
2. Document findings in `code-review-notes.md`:
   - Code quality issues
   - Security concerns
   - Best practices violations
   - Performance improvements
3. Apply accepted fixes
4. Document rejected suggestions with reasoning in `review-fixes.md`
5. Re-run tests after changes
6. Verify no new issues introduced

**Deliverables:** Refined codebase, code review documentation, updated tests

### Phase 10: Documentation & Finalization
**Goal:** Complete all required documentation artifacts.

1. **README.md** - Complete setup instructions:
   - Prerequisites (.NET 9 SDK, SQL Server)
   - Connection string configuration
   - Database migration commands
   - Running API and Blazor projects
   - Running tests
2. **pr-description.md** - Summary as if for PR review
3. **reflection.md** - Honest assessment of AI usage (what worked, what didn't, validation approach)
4. **final-ai-usage-summary.md** - Overall AI workflow summary
5. Verify all `ai-prompts/` files are complete with iterations
6. Ensure no secrets in repository

**Deliverables:** Complete documentation package, submission-ready repository

### Phase 11: Stretch Features (Optional)
**Goal:** Demonstrate advanced capabilities if time permits.

Options (prioritize based on available time):
1. **Authentication:** JWT-based auth, protected routes, role-based access
2. **User Management:** User CRUD, role assignment
3. **Advanced Filtering:** Filter by priority/assignee, sorting, pagination
4. **Additional Testing:** Edge cases, failure scenarios, performance tests
5. **DevOps:** Docker containers, GitHub Actions CI, Swagger documentation

**Note:** Stretch features count toward C1.1 evidence but Core quality is more important.

## Milestones

### Milestone 1: Foundation Complete (Day 1-2)
- [ ] Project structure created
- [ ] Git repository initialized
- [ ] Cursor context configured
- [ ] tool-workflow.md (Part A) completed
- [ ] All planning artifact templates created
- [ ] Database designed and verified for persistence

**Success Criteria:** Can create a ticket, restart the API, and ticket still exists

### Milestone 2: Backend Complete (Day 3-4)
- [ ] State machine implemented and unit tested
- [ ] All API endpoints functional
- [ ] Input validation working
- [ ] Error handling in place
- [ ] State machine integration tests passing

**Success Criteria:** All state transitions enforce rules correctly via API

### Milestone 3: Frontend Complete (Day 5)
- [ ] All Blazor pages implemented
- [ ] Status transitions work in UI
- [ ] Search and filter functional
- [ ] Comments work end-to-end
- [ ] Error states display properly

**Success Criteria:** Can perform all Core operations through UI

### Milestone 4: Testing Complete (Day 6)
- [ ] All mandatory tests written and passing
- [ ] Manual testing complete
- [ ] Issues fixed and documented
- [ ] Code review performed
- [ ] Refinements applied

**Success Criteria:** Application meets all Core acceptance criteria

### Milestone 5: Documentation Complete (Day 7)
- [ ] README with setup instructions
- [ ] All prompt history documented
- [ ] Debugging notes complete
- [ ] Code review notes complete
- [ ] Reflection written
- [ ] PR description prepared
- [ ] Repository submission-ready

**Success Criteria:** All required artifacts present and complete

## AI Usage Plan

### Context Provision Strategy
1. **Initial Setup:** Create `.cursorrules` with project-specific conventions before any code generation
2. **Persistent Context:** Maintain `project-context.md` and `spec.md` in cursor-workflow folder
3. **Task Context:** Reference specific files/components when prompting: "In TicketSystem.Api/Services/TicketStateMachine.cs..."
4. **Iterative Refinement:** Use follow-up prompts to refine AI output: "Refine this to handle edge case X", "Add validation for Y"

### AI Usage by Lifecycle Phase

**Requirement Analysis:**
- Extract and organize requirements from spec
- Identify edge cases and assumptions
- Generate acceptance criteria drafts
- Document in `ai-prompts/planning.md`

**Design:**
- Generate architecture diagrams (Mermaid syntax)
- Design entity relationships
- Create API endpoint specifications
- State machine logic design
- Document in `ai-prompts/design.md`

**Implementation:**
- Generate boilerplate: DbContext, entities, controllers, services
- Implement state machine logic with validation
- Create Blazor components and pages
- Generate DTOs and validators
- Document all prompts with iterations in `ai-prompts/implementation.md`

**Testing:**
- Generate test scaffolding
- Create state machine test cases (all transitions)
- Generate edge case tests
- Document in `ai-prompts/testing.md`

**Debugging:**
- Analyze error messages
- Suggest debugging approaches
- Propose fixes for issues
- Validate fix logic
- Document in `ai-prompts/debugging.md`

**Code Review:**
- AI-assisted code review for quality, security, best practices
- Refactoring suggestions
- Performance optimization ideas
- Document in `ai-prompts/code-review.md`

**Documentation:**
- Generate README structure
- Create API documentation comments
- Draft PR descriptions
- Document in `ai-prompts/documentation.md`

### Validation Approach
- **Always validate AI output** - don't blindly accept
- Test generated code before committing
- Verify state machine logic against specification
- Review security implications of suggested code
- Document validation steps and corrections
- Capture mistakes and how they were caught

### Information Boundaries
- **No secrets:** Never include API keys, passwords, connection strings with credentials
- **No proprietary patterns:** Keep business logic generic
- **Public code only:** All generated code suitable for public repository

### Reusable Assets
- `.cursorrules` file for future .NET projects
- Prompt templates in `ai-prompts/` folders
- Project context structure in `cursor-workflow/`
- Test generation patterns
- Documentation templates

## Risks & Mitigation

### Risk 1: State Machine Logic Complexity
**Impact:** High - This is the signature piece that demonstrates engineering judgment  
**Likelihood:** Medium - Complex business logic prone to edge case bugs  
**Mitigation:**
- Implement state machine early (Phase 5.2)
- Write unit tests before service integration
- Create integration tests for all valid and invalid transitions
- Manual verification through UI testing
- AI-assisted review of transition logic

### Risk 2: Time Management
**Impact:** High - Only one week for full project + extensive documentation  
**Likelihood:** High - Documentation represents 60% of evaluation  
**Mitigation:**
- Focus on Core features first; defer Stretch features
- Document as you go, not at the end
- Use AI to draft documentation but personalize
- Prioritize state machine tests over optional tests
- Track progress against daily milestones

### Risk 3: Database Persistence Verification
**Impact:** Medium - Core requirement that data survives restart  
**Likelihood:** Low - Straightforward if set up correctly  
**Mitigation:**
- Verify persistence immediately after Phase 4 (database setup)
- Document verification steps in database/setup-notes.md
- Test early to catch configuration issues
- Include persistence check in integration tests

### Risk 4: SQL Server Setup Complexity
**Impact:** Medium - Could block progress if setup fails  
**Likelihood:** Medium - SQL Server installation and configuration can have issues  
**Mitigation:**
- Use SQL Server LocalDB for simplicity
- Document setup steps clearly in README and setup-notes.md
- Test connection string configuration early
- Have SQLite in-memory option as fallback for development

### Risk 5: Blazor WebAssembly CORS Issues
**Impact:** Medium - Could prevent frontend from calling API  
**Likelihood:** Medium - Common issue in Blazor WebAssembly development  
**Mitigation:**
- Configure CORS in API startup early (Phase 5.1)
- Test API from Blazor immediately after first endpoint implementation
- Document CORS configuration in README
- Use clear error messages for debugging

### Risk 6: Incomplete Prompt History Documentation
**Impact:** High - Prompt history represents major evaluation component  
**Likelihood:** Medium - Easy to forget to document during active development  
**Mitigation:**
- Create `ai-prompts/` files at start of each phase
- Document prompts immediately after use, not later
- Include context, AI response summary, what was accepted/rejected, why
- Set reminders to update prompt history
- Use AI to help format prompt documentation

### Risk 7: Test Coverage Gaps
**Impact:** Medium - State machine integration tests are mandatory  
**Likelihood:** Low - Clear requirements specified  
**Mitigation:**
- Write state machine tests immediately after implementation
- Create test checklist from acceptance criteria
- Run tests frequently during development
- Document test results in test-results.md
- Use AI to generate additional edge case tests

### Risk 8: Copy-Paste Without Understanding
**Impact:** High - Evaluation focuses on understanding and validation  
**Likelihood:** Medium - Temptation to quickly accept AI suggestions  
**Mitigation:**
- Always review and understand AI-generated code
- Test all generated code
- Document what was changed and why
- Capture mistakes and corrections in debugging-notes.md
- Show iteration in prompt history, not one-shot prompts
- Include validation prompts in ai-prompts files

### Risk 9: Missing Documentation Artifacts
**Impact:** High - 60% of evaluation focuses on lifecycle artifacts  
**Likelihood:** Medium - Many files required across different phases  
**Mitigation:**
- Create all documentation templates in Phase 3
- Use repository structure checklist
- Review required artifacts list before final submission
- Set aside dedicated time for documentation (Day 7)
- Use AI to draft but ensure personalization

### Risk 10: Secrets Committed to Repository
**Impact:** Critical - Automatic disqualification concern  
**Likelihood:** Low - Can be prevented with proper .gitignore  
**Mitigation:**
- Create comprehensive .gitignore in Phase 1
- Use environment variables for connection strings
- Review commits before pushing
- Use example configuration files with placeholder values
- Final security check before submission

## Success Criteria Summary

### Technical Completeness
- Working Blazor frontend with all Core features
- Working ASP.NET Core API with validation and error handling
- SQL Server database with migrations and seed data
- State machine enforces all valid and invalid transitions
- Search and filter functionality works
- Data persists across application restarts
- State machine integration tests pass
- No secrets in repository

### Lifecycle Artifacts Completeness (60% of evaluation)
- tool-workflow.md (Part A) demonstrates AI workflow foundation
- All planning documents complete (requirements, acceptance, design, API, data model, UI flow)
- Complete prompt history in ai-prompts/ showing iteration and validation
- Test strategy and results documented
- Debugging notes with AI usage and validation
- Code review notes with AI assistance and critical evaluation
- PR description suitable for team review
- Honest reflection on what worked and what didn't
- Cursor workflow documentation in tool-specific/

### AI Workflow Evidence
- Prompt history shows iteration, not copy-paste
- Context-setting approach is clear and consistent
- Validation of AI output is documented
- Mistakes and corrections are captured
- Reusable assets created (.cursorrules, project context, prompt templates)
- Information boundaries respected (no secrets)

---

**Note:** This is a development exercise, not a test. The goal is to demonstrate and grow AI-assisted development capability across the full software lifecycle. Quality of thought process and documentation is as important as the working application.
