# Tasks Breakdown

## Task Priority Legend
- 🔴 Critical - Must complete for Core
- 🟡 Important - Should complete for quality
- 🟢 Optional - Stretch features

---

## Phase 1: Foundation & Setup ✓

- [x] Create project directory structure
- [x] Initialize Git repository with .gitignore
- [x] Create .NET solution with three projects
- [x] Create all documentation file templates
- [x] Set up folder hierarchy

**Status:** COMPLETED  
**Date Completed:** 2026-07-13

---

## Phase 2: Cursor Context & AI Workflow (Part A) ✓

- [x] Create .cursorrules file with conventions
- [x] Create tool-specific/cursor-workflow/ files
- [x] Write tool-workflow.md (Part A)

**Status:** COMPLETED  
**Date Completed:** 2026-07-13

---

## Phase 3: Requirement Analysis & Planning 🔴

- [ ] Complete candidate-info.md (personal details)
- [ ] Complete requirements-analysis.md (Understanding and Assumptions sections)
- [ ] Review and finalize acceptance-criteria.md
- [ ] Review design-notes.md structure
- [ ] Review api-contract.md structure
- [ ] Review data-model.md
- [ ] Review ui-flow.md
- [ ] Review test-strategy.md

**Status:** TODO  
**Dependencies:** None  
**Estimated Time:** 2-3 hours

---

## Phase 4: Database Design & Implementation 🔴

### Database Schema
- [ ] Create User entity
- [ ] Create Ticket entity with all fields
- [ ] Create Comment entity
- [ ] Configure relationships in DbContext
- [ ] Apply Fluent API configurations

### Migrations
- [ ] Create initial migration
- [ ] Create seed data for 3-5 users
- [ ] Apply migrations to create database

### Verification
- [ ] Test database connection
- [ ] Verify tables created
- [ ] Verify seed data loaded
- [ ] **CRITICAL:** Create a ticket, restart API, verify persistence

### Documentation
- [ ] Complete database/setup-notes.md with actual setup steps
- [ ] Document connection string configuration
- [ ] Document verification results

**Status:** TODO  
**Dependencies:** Phase 2 complete  
**Estimated Time:** 3-4 hours

---

## Phase 5: Backend API Implementation 🔴

### 5.1: Core Infrastructure
- [ ] Create entity classes (User, Ticket, Comment)
- [ ] Create DTOs (CreateTicketDto, UpdateTicketDto, TicketResponseDto, etc.)
- [ ] Create AppDbContext
- [ ] Configure dependency injection in Program.cs
- [ ] Create global exception handling middleware
- [ ] Configure CORS for Blazor

### 5.2: State Machine (CRITICAL) 🔴
- [ ] Create ITicketStateMachine interface
- [ ] Implement TicketStateMachine class
- [ ] Implement CanTransition method
- [ ] Implement ValidateTransition method
- [ ] Add unit tests for state machine logic
- [ ] Verify all 5 valid transitions
- [ ] Verify all invalid transitions throw/return false

### 5.3: Services Layer
- [ ] Create ITicketService and TicketService
- [ ] Implement GetAllTickets (with optional filters)
- [ ] Implement GetTicketById
- [ ] Implement CreateTicket
- [ ] Implement UpdateTicket
- [ ] Implement ChangeTicketStatus (with state machine validation)
- [ ] Create ICommentService and CommentService
- [ ] Implement AddComment
- [ ] Implement GetCommentsByTicket

### 5.4: Validators
- [ ] Create ticket validators (title, description, priority)
- [ ] Create comment validators
- [ ] Add validation attributes or FluentValidation

### 5.5: Controllers
- [ ] Create TicketsController
  - [ ] GET /api/tickets
  - [ ] GET /api/tickets/{id}
  - [ ] POST /api/tickets
  - [ ] PUT /api/tickets/{id}
  - [ ] PUT /api/tickets/{id}/status
  - [ ] GET /api/tickets/search
- [ ] Create CommentsController
  - [ ] POST /api/tickets/{ticketId}/comments
  - [ ] GET /api/tickets/{ticketId}/comments
- [ ] Create UsersController (read-only)
  - [ ] GET /api/users
  - [ ] GET /api/users/{id}

**Status:** TODO  
**Dependencies:** Phase 4 complete  
**Estimated Time:** 6-8 hours

---

## Phase 6: Frontend Implementation 🔴

### 6.1: HTTP Services
- [ ] Create ITicketApiService interface
- [ ] Implement TicketApiService with HttpClient
- [ ] Create ICommentApiService interface
- [ ] Implement CommentApiService
- [ ] Configure HttpClient base address
- [ ] Add error handling to API services

### 6.2: Pages
- [ ] Create TicketList.razor
  - [ ] Display all tickets
  - [ ] Add search box
  - [ ] Add status filter dropdown
  - [ ] Handle empty state
- [ ] Create TicketDetail.razor
  - [ ] Display ticket information
  - [ ] Show comments list
  - [ ] Add comment form
  - [ ] Show status transition buttons (valid ones only)
  - [ ] Handle status change
- [ ] Create CreateTicket.razor
  - [ ] Form with title, description, priority, assignee
  - [ ] Form validation
  - [ ] Submit handler
- [ ] Create EditTicket.razor
  - [ ] Pre-populate form
  - [ ] Update handler

### 6.3: Reusable Components
- [ ] Create StatusBadge.razor (color-coded status)
- [ ] Create PriorityBadge.razor (color-coded priority)
- [ ] Create CommentList.razor
- [ ] Create AddComment.razor
- [ ] Create SearchFilter.razor

### 6.4: UI/UX
- [ ] Loading indicators for async operations
- [ ] Error message display
- [ ] Success notifications
- [ ] Responsive design (Bootstrap)
- [ ] Navigation between pages

**Status:** TODO  
**Dependencies:** Phase 5 complete  
**Estimated Time:** 5-7 hours

---

## Phase 7: Testing Implementation 🔴

### 7.1: State Machine Tests (MANDATORY)
- [ ] Test: Open → InProgress succeeds
- [ ] Test: Open → Cancelled succeeds
- [ ] Test: InProgress → Resolved succeeds
- [ ] Test: InProgress → Cancelled succeeds
- [ ] Test: Resolved → Closed succeeds
- [ ] Test: Open → Resolved returns 400
- [ ] Test: Open → Closed returns 400
- [ ] Test: Resolved → InProgress returns 400
- [ ] Test: Closed → any status returns 400
- [ ] Test: Cancelled → any status returns 400
- [ ] Test: Status change persists in database

### 7.2: Integration Tests
- [ ] Setup WebApplicationFactory
- [ ] Setup SQLite in-memory database
- [ ] Test: Create ticket
- [ ] Test: Get all tickets
- [ ] Test: Get ticket by ID
- [ ] Test: Update ticket
- [ ] Test: Add comment
- [ ] Test: Search tickets
- [ ] Test: Filter tickets by status

### 7.3: Validation Tests
- [ ] Test: Create ticket with missing title
- [ ] Test: Create ticket with long title
- [ ] Test: Create comment with empty message
- [ ] Test: Invalid priority value

### 7.4: Documentation
- [ ] Complete test-strategy.md
- [ ] Run all tests and capture results
- [ ] Complete test-results.md with actual results

**Status:** TODO  
**Dependencies:** Phase 5 & 6 complete  
**Estimated Time:** 4-5 hours

---

## Phase 8: Testing & Debugging 🟡

- [ ] Run full test suite
- [ ] Fix any failing tests
- [ ] Manual UI testing
- [ ] Test state machine transitions through UI
- [ ] Test edge cases
- [ ] Document all issues in debugging-notes.md
- [ ] Document how AI helped with debugging
- [ ] Verify data persistence manually

**Status:** TODO  
**Dependencies:** Phase 7 complete  
**Estimated Time:** 3-4 hours

---

## Phase 9: Code Review & Refinement 🟡

- [ ] Use AI for code review
- [ ] Document findings in code-review-notes.md
- [ ] Apply accepted fixes
- [ ] Document rejected suggestions in review-fixes.md
- [ ] Re-run tests after changes
- [ ] Check for security issues
- [ ] Check for best practice violations

**Status:** TODO  
**Dependencies:** Phase 8 complete  
**Estimated Time:** 2-3 hours

---

## Phase 10: Documentation & Finalization 🔴

- [ ] Complete README.md with setup instructions
- [ ] Verify database/setup-notes.md is complete
- [ ] Complete pr-description.md
- [ ] Complete reflection.md with honest assessment
- [ ] Complete final-ai-usage-summary.md
- [ ] Verify all ai-prompts/ files are complete
- [ ] Review all lifecycle documents for completeness
- [ ] Ensure no secrets in repository
- [ ] Test setup instructions from README

**Status:** TODO  
**Dependencies:** Phase 9 complete  
**Estimated Time:** 4-5 hours

---

## Phase 11: Stretch Features (Optional) 🟢

Choose based on remaining time:

### Option A: Authentication
- [ ] Add JWT authentication
- [ ] Login/logout endpoints
- [ ] Protected routes in API
- [ ] Auth state in Blazor

### Option B: Advanced Filtering
- [ ] Filter by priority
- [ ] Filter by assignee
- [ ] Sorting options
- [ ] Pagination

### Option C: Additional Testing
- [ ] Unit tests for all services
- [ ] Edge case tests
- [ ] Concurrency tests

### Option D: DevOps
- [ ] Dockerfile for API
- [ ] Dockerfile for Blazor
- [ ] Docker Compose
- [ ] GitHub Actions workflow

**Status:** TODO  
**Dependencies:** All Core phases complete  
**Estimated Time:** Varies (2-8 hours depending on feature)

---

## Continuous Tasks (Throughout Project)

### AI Prompt Documentation 🔴
- [ ] Document planning prompts in ai-prompts/planning.md
- [ ] Document design prompts in ai-prompts/design.md
- [ ] Document implementation prompts in ai-prompts/implementation.md
- [ ] Document testing prompts in ai-prompts/testing.md
- [ ] Document debugging prompts in ai-prompts/debugging.md
- [ ] Document code review prompts in ai-prompts/code-review.md
- [ ] Document documentation prompts in ai-prompts/documentation.md

**Note:** Document prompts AS YOU GO, not at the end!

### Code Quality
- [ ] Run linter regularly
- [ ] Fix warnings as they appear
- [ ] Keep code formatted consistently
- [ ] Commit frequently with clear messages

---

## Time Allocation (Recommended)

| Phase | Time | Day |
|-------|------|-----|
| 1-2: Setup & Context | 2-3 hours | Day 1 |
| 3: Planning | 2-3 hours | Day 1 |
| 4: Database | 3-4 hours | Day 2 |
| 5: Backend API | 6-8 hours | Days 2-3 |
| 6: Frontend | 5-7 hours | Day 4-5 |
| 7: Testing | 4-5 hours | Day 5-6 |
| 8: Debugging | 3-4 hours | Day 6 |
| 9: Code Review | 2-3 hours | Day 6 |
| 10: Documentation | 4-5 hours | Day 7 |
| 11: Stretch (optional) | Variable | Day 7 |

**Total Core Time:** ~35-45 hours  
**Total with Stretch:** ~40-55 hours

---

## Progress Tracking

**Overall Progress:** [X] / [Total] tasks completed ([X]%)

**Phase Status:**
- Phase 1: ✅ Complete
- Phase 2: ✅ Complete
- Phase 3: ⏳ In Progress / 📝 TODO
- Phase 4: 📝 TODO
- Phase 5: 📝 TODO
- Phase 6: 📝 TODO
- Phase 7: 📝 TODO
- Phase 8: 📝 TODO
- Phase 9: 📝 TODO
- Phase 10: 📝 TODO
- Phase 11: 📝 Optional

**Last Updated:** 2026-07-16
