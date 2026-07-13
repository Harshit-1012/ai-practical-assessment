# AI Tool Workflow (Part A)

## Overview

This document describes how I used AI tools throughout the software development lifecycle for the Support Ticket Management System project. This is Part A of the .NET AI Capability Exercise.

---

## 1. Primary AI Tool Used

**Tool Name:** Cursor AI  
**Platform:** Integrated AI assistant in Cursor IDE (VS Code-based)  
**Model:** [Specific model version used]  
**Why Chosen:** Integrated into development environment, context-aware, supports .NET and C# effectively

---

## 2. How I Provide Project Context to the Tool

### Strategy: Multi-Layered Context Provision

I provide context to Cursor AI through several mechanisms to ensure it understands the project requirements and conventions:

#### A. .cursorrules File
**Location:** `.cursorrules` in project root  
**Purpose:** Define project-wide coding standards, conventions, and patterns

**Contents:**
- .NET 9 and C# 12 conventions
- ASP.NET Core Web API patterns
- Entity Framework Core best practices
- Blazor WebAssembly patterns
- Naming conventions
- Code organization standards
- State machine requirements (critical validation rules)
- Error handling patterns
- Testing standards
- Security best practices

**Why This Helps:**
Cursor AI automatically reads the .cursorrules file and applies these conventions to all code generation, ensuring consistency without repeating context in every prompt.

#### B. Project Context Document
**Location:** `tool-specific/cursor-workflow/project-context.md`  
**Purpose:** High-level project overview and goals

**Contents:**
- Project purpose and business context
- Technical stack (Blazor, ASP.NET Core, SQL Server)
- Core features and requirements
- Key constraints (time, technical, documentation)
- Entity definitions
- Success criteria
- Risk areas
- AI tool usage strategy

**How I Use It:**
Reference this document when asking high-level questions or when starting a new major feature. Helps AI understand the "why" behind requirements.

#### C. Specification Document
**Location:** `tool-specific/cursor-workflow/spec.md`  
**Purpose:** Complete requirements specification

**Contents:**
- Detailed project requirements
- Acceptance criteria
- State machine rules
- Mandatory vs. optional features
- Repository structure requirements
- Documentation requirements

**How I Use It:**
Point AI to this when extracting requirements, designing features, or validating implementations.

#### D. Task Breakdown
**Location:** `tool-specific/cursor-workflow/tasks.md`  
**Purpose:** Detailed task list by phase

**Contents:**
- Phase-by-phase task breakdown
- Priority indicators (critical/important/optional)
- Dependencies between tasks
- Time estimates
- Progress tracking

**How I Use It:**
Helps AI understand current phase, what's already done, and what comes next.

#### E. Inline File Context
**Method:** Reference specific files and code in prompts

**Example:**
```
"In src/TicketSystem.Api/Services/TicketStateMachine.cs, add validation logic 
for the state machine that enforces these transitions: [list]"
```

**Why This Helps:**
Provides precise context about where code should go and what exists already.

---

## 3. How I Use AI Across the Lifecycle

### A. Requirement Analysis (Phase 3)

**Activities:**
- Extract functional requirements from spec.md
- Identify non-functional requirements
- Generate edge case scenarios
- Create acceptance criteria checklists

**Typical Prompts:**
```
"Review spec.md and extract all functional requirements for the ticket 
management system. Organize by feature area."

"Based on the state machine requirements in spec.md, list all edge cases 
that need to be handled."

"Generate acceptance criteria for the comment system feature."
```

**How I Validate:**
- Compare AI output against spec.md
- Ensure no requirements missed
- Add my own understanding and assumptions
- Verify edge cases make sense

**Output:**
- requirements-analysis.md (with my personal understanding added)
- acceptance-criteria.md (reviewed and customized)

**Prompt Documentation:**
All prompts documented in `ai-prompts/planning.md` with iterations

---

### B. Planning and Design (Phase 3)

**Activities:**
- Generate architecture diagrams (Mermaid syntax)
- Design database schema with relationships
- Create API endpoint specifications
- Design state machine logic
- Plan UI component hierarchy

**Typical Prompts:**
```
"Create a Mermaid entity relationship diagram for User, Ticket, and Comment 
entities based on requirements in spec.md"

"Design an API contract for ticket CRUD operations. Include request/response 
formats, validation rules, and error codes."

"Design a state machine class structure that enforces these transitions: [list]. 
Include interface and implementation with clear validation."
```

**How I Validate:**
- Verify diagrams match requirements
- Check that API design follows REST principles
- Ensure state machine rules are complete and correct
- Review for missing scenarios

**Output:**
- design-notes.md with architecture decisions
- api-contract.md with endpoint specs
- data-model.md with entity definitions
- Mermaid diagrams for ERD and state machine

**Prompt Documentation:**
All prompts documented in `ai-prompts/design.md`

---

### C. Code Generation (Phases 4-6)

**Activities:**

#### Database (Phase 4)
- Generate entity classes
- Create DbContext with Fluent API configurations
- Generate migrations
- Create seed data

**Example Prompt:**
```
"Create an EF Core DbContext class for the ticket system with User, Ticket, 
and Comment entities. Use Fluent API to configure relationships. Follow 
conventions in .cursorrules file."
```

#### Backend API (Phase 5)
- Generate controllers with CRUD endpoints
- Implement service layer with business logic
- Create TicketStateMachine class (critical piece)
- Generate DTOs and validators
- Implement error handling middleware

**Example Prompt:**
```
"Create a TicketStateMachine class that implements ITicketStateMachine. 
It should have CanTransition(from, to) returning bool and ValidateTransition(from, to) 
throwing exception for invalid transitions. Valid transitions are: 
Open->InProgress, Open->Cancelled, InProgress->Resolved, InProgress->Cancelled, 
Resolved->Closed."
```

#### Frontend (Phase 6)
- Generate Blazor pages and components
- Create HTTP service wrappers
- Implement forms with validation
- Create reusable UI components

**Example Prompt:**
```
"Create a Blazor page at /tickets that displays all tickets in a Bootstrap card 
layout. Include a search box and status filter dropdown. Inject ITicketApiService 
and handle loading states and errors."
```

**How I Validate:**
- Read every line of generated code
- Test all generated methods
- Verify state machine logic manually
- Run code to check for compilation errors
- Test edge cases
- Check for security issues

**What I Typically Change:**
- Error messages (make more user-friendly)
- Validation logic (add edge cases AI missed)
- State machine logic (AI sometimes misses invalid transitions)
- UI styling (adjust Bootstrap classes)

**Output:**
- Complete backend API implementation
- Complete Blazor frontend
- Working state machine with validation

**Prompt Documentation:**
All prompts documented in `ai-prompts/implementation.md` with:
- What was accepted as-is
- What was modified
- What was rejected
- Why changes were made

---

### D. Testing (Phase 7)

**Activities:**
- Generate unit test structure
- Create state machine tests (all transitions)
- Generate integration tests for API endpoints
- Create test data and helpers
- Generate edge case tests

**Typical Prompts:**
```
"Create xUnit integration tests for the state machine. Test all 5 valid 
transitions succeed (return 200) and all invalid transitions fail (return 400). 
Use WebApplicationFactory and SQLite in-memory database."

"Generate unit tests for TicketStateMachine.CanTransition() method. Test 
every possible from-to status combination. Use [Fact] attribute and descriptive 
test names."

"Create test data helpers that seed a test database with sample users, tickets, 
and comments."
```

**How I Validate:**
- Run all generated tests
- Verify test names are descriptive
- Check that tests are independent
- Ensure edge cases are covered
- Verify test data is realistic

**What I Typically Add:**
- Additional edge case tests AI missed
- More realistic test data
- Better assertion messages
- Cleanup logic if needed

**Output:**
- Comprehensive test suite
- State machine tests (mandatory - all transitions covered)
- Integration tests for API
- Validation tests

**Prompt Documentation:**
All prompts documented in `ai-prompts/testing.md`

---

### E. Debugging (Phase 8)

**Activities:**
- Analyze error messages
- Identify root causes
- Get suggestions for fixes
- Validate fix logic

**Typical Prompts:**
```
"I'm getting this error when trying to change ticket status: [error message]. 
Here's the relevant code: [code]. What's causing this and how do I fix it?"

"The state machine is allowing Open -> Resolved transition which should be invalid. 
Review this code and identify the bug: [code]"

"This integration test is failing: [test name]. Expected 400 but got 200. 
Here's the test code: [code]. What's wrong?"
```

**How I Validate AI Suggestions:**
- Reproduce the error myself
- Test the suggested fix
- Verify the fix doesn't break other functionality
- Ensure the root cause is actually fixed (not just symptoms)
- Check if similar bugs exist elsewhere

**What I Document:**
- The problem
- AI's diagnosis
- Whether AI was correct
- The actual fix applied
- How I validated the fix

**Output:**
- Fixed bugs and issues
- debugging-notes.md with detailed issue documentation

**Prompt Documentation:**
All prompts documented in `ai-prompts/debugging.md`

---

### F. Code Review (Phase 9)

**Activities:**
- Request AI code review
- Security assessment
- Best practices validation
- Performance considerations
- Refactoring suggestions

**Typical Prompts:**
```
"Review src/TicketSystem.Api/Services/TicketService.cs for code quality issues, 
potential bugs, security concerns, and best practices violations."

"Analyze the state machine implementation for logic errors and edge cases 
that might not be handled."

"Review the entire API project for security vulnerabilities including SQL 
injection, XSS, improper error handling, and secrets in code."
```

**How I Validate AI Findings:**
- Assess severity of each issue
- Verify issues are real (not false positives)
- Determine if suggested fixes are appropriate
- Consider trade-offs of suggestions

**What I Document:**
- Issues found and accepted
- Changes made in response
- Suggestions rejected and why
- Trade-offs considered

**Output:**
- Improved code quality
- Security issues fixed
- code-review-notes.md with findings
- review-fixes.md with changes and rationale

**Prompt Documentation:**
All prompts documented in `ai-prompts/code-review.md`

---

### G. Documentation (Phase 10)

**Activities:**
- Generate README content
- Create API documentation
- Draft PR descriptions
- Generate code comments
- Create user-facing documentation

**Typical Prompts:**
```
"Generate a README.md for this project with setup instructions, prerequisites, 
how to run the API, how to run the Blazor app, and how to run tests."

"Create API documentation comments for all public methods in TicketsController."

"Draft a pull request description for this feature. Summarize what was built, 
technical approach, testing done, and known limitations."
```

**How I Personalize AI Output:**
- Add my own voice and perspective
- Include specific observations from my experience
- Remove generic content
- Add concrete examples
- Reflect honestly on challenges

**What I Never Accept As-Is:**
- Reflection documents (must be personal)
- Honest assessments (AI tends to be overly positive)
- Learning summaries (must reflect my actual learnings)

**Output:**
- Complete README with setup instructions
- API documentation
- PR description
- All lifecycle documents complete

**Prompt Documentation:**
All prompts documented in `ai-prompts/documentation.md`

---

## 4. What Information I Avoid Sharing

### Security and Privacy
❌ **Never Share:**
- Passwords or credentials
- API keys or tokens
- Real connection strings with actual credentials
- Production database connection strings
- Personal identification information
- Proprietary business logic patterns (not applicable for this assessment project)

✓ **Safe to Share:**
- Example connection strings with placeholders
- Public code and algorithms
- Generic business requirements
- Common design patterns
- Open-source libraries and their usage

### Project-Specific Boundaries
For this assessment project:
- All code is meant to be shared (it's a public portfolio piece)
- Requirements are generic (ticket management)
- No real user data involved
- No proprietary algorithms

In a real project, I would also avoid:
- Company-specific business rules
- Proprietary algorithms
- Customer data
- Internal architecture details of production systems

---

## 5. How I Would Reuse This Workflow

### Reusable Assets Created

#### 1. .cursorrules File
**Reusability:** High  
**How to Reuse:**
- Copy to new .NET 9 projects
- Modify specific conventions as needed
- Add project-specific patterns
- Keep core standards (naming, async, error handling)

**Value:**
Ensures consistent code generation across projects without repeating context.

#### 2. Project Context Template
**Reusability:** High  
**How to Reuse:**
- Use `project-context.md` structure for any project
- Fill in with new project details
- Adapt sections to project type
- Keep sections: Overview, Stack, Entities, Constraints, Risks

**Value:**
Provides AI with comprehensive project understanding from the start.

#### 3. Prompt Templates
**Reusability:** Medium to High  
**Location:** `ai-prompts/*.md`  
**How to Reuse:**
- Use prompt patterns from each lifecycle phase
- Adapt specific details to new project
- Keep the structure of providing context + instruction + constraints
- Reuse patterns like "Review X for Y issues"

**Value:**
Proven prompt patterns that get good results.

#### 4. Documentation Templates
**Reusability:** High  
**How to Reuse:**
- All markdown templates created (requirements, design, test strategy, etc.)
- Use as starting point for any project
- Adapt sections to project needs
- Keep structured approach

**Value:**
Saves time on documentation structure planning.

#### 5. Test Strategy Patterns
**Reusability:** Medium  
**How to Reuse:**
- State machine test matrix approach
- Integration test structure with WebApplicationFactory
- Test naming conventions
- Arrange-Act-Assert patterns

**Value:**
Systematic approach to testing complex business logic.

### Workflow Steps to Repeat

**For Any Future Project:**

1. **Setup Phase:**
   - Create .cursorrules file with project conventions early
   - Create project-context.md
   - Copy spec/requirements to cursor-workflow/ folder
   - Create tasks.md with phase breakdown

2. **Implementation Phase:**
   - Reference context in prompts: "In [file], following .cursorrules..."
   - Iterate on AI output, don't accept first version
   - Test generated code immediately
   - Document prompts in ai-prompts/ as you go

3. **Validation Phase:**
   - Read all AI-generated code
   - Test functionality
   - Check for security issues
   - Verify edge cases

4. **Documentation Phase:**
   - Use AI to draft but always personalize
   - Document what AI got wrong (honest assessment)
   - Show iteration in prompt history

5. **Review Phase:**
   - AI-assisted code review
   - Document accepted and rejected suggestions
   - Explain reasoning for decisions

### Key Success Factors

✓ **Provide Rich Context:** .cursorrules, project-context, spec references  
✓ **Iterate:** Don't accept first AI output, refine and improve  
✓ **Validate:** Test everything, check logic, verify against requirements  
✓ **Document:** Track prompts, decisions, and learnings as you go  
✓ **Personalize:** Make AI output your own, especially documentation  
✓ **Be Honest:** Document mistakes (AI's and yours) for learning

---

## Summary

This workflow demonstrates:
1. **Strategic Context Provision** - Multiple layers of context ensure AI understands the project
2. **Lifecycle Coverage** - AI used from requirements through documentation
3. **Validation-First Approach** - All AI output is validated, not blindly accepted
4. **Iterative Refinement** - Showing iteration and improvement, not copy-paste
5. **Honest Documentation** - Capturing what worked and what didn't
6. **Reusable Patterns** - Creating assets that can be used in future projects

The goal is not to have AI write all the code, but to accelerate development while maintaining quality and demonstrating engineering judgment through validation and refinement.

---

**Document Created By:** [Your Name]  
**Date:** [Date]  
**Tool Used:** Cursor AI  
**Purpose:** Part A of .NET AI Capability Exercise
