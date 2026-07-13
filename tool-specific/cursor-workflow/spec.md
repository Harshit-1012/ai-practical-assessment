# .NET AI Capability Exercise — Project Spec (Support Ticket Management System, Cursor)

Participant Guide — Build & Grow Your AI Workflow
A hands-on exercise every developer in the .NET competency completes to strengthen and show how they work with AI. This produces a feedback report and a personalized growth path — it is for development, not a graded test.

## 1. What This Is
This is a hands-on capability exercise to help you develop — and make visible — how you use AI tools effectively, responsibly, and practically across the software development lifecycle. Everyone in the competency takes part; it is a shared part of how we build AI capability, not something a few people are singled out for.

It is not a graded test. You will build a small full-stack project and show your thinking, and in return, you get a feedback report and a clear sense of what to grow next. What matters is not only whether the final app works, but how you used AI for requirement analysis, planning, implementation, testing, debugging, code review, documentation, and reflection. Making your thinking visible is the point.

## 2. Who Takes Part
All developers in the competency, from SE to TL level, working with stacks such as .NET, or full-stack combinations. Because everyone does it, there is a shared baseline, and no one is measured against a different bar than their peers.

## 3. Time and Effort
Self-paced, meant to be completed within one week. Work in any order; there is no required day-wise plan. Share your work by the agreed date.

- **Expected effort:** The mandatory Core is scoped for roughly 8–12 focused hours.
- **Lifecycle Artifacts:** The rest of the week goes into requirement analysis, prompt history, testing/debugging notes, and reflection — the main things the feedback looks at.
- **Important:** Do not expand the Core application at the expense of these artifacts.

## 4. What You Get Out of It
A feedback report detailing strengths, growth areas, and concrete next steps for developing your AI workflow — a snapshot and direction, not a grade.

Feedback focuses on: requirement analysis, prompting and context-setting, tool workflow, full-stack design, code quality, database design, testing depth, debugging, code review, documentation, ownership, and responsible AI judgment.

## 5. How the Exercise is Structured

| Part | Focus | Emphasis |
|---|---|---|
| Part A | AI Workflow Foundation | 20% |
| Part B | Full-Stack Mini Project (Core + optional Stretch) | 60% |
| Part C | Submission and Reflection | 20% |

Percentages indicate where to put effort — not exam weights.

### Part A: AI Workflow Foundation
Submit `tool-workflow.md` covering:
1. Primary AI tool used.
2. How you provide project context to the tool.
3. How you use AI for requirement analysis, planning/design, code generation, validation, testing, debugging, and code review.
4. What information you avoid sharing unnecessarily with AI tools.
5. How you would reuse this workflow in a real project.

### Part B: Full-Stack Mini Project
Demonstrate practical AI-assisted delivery. Core is mandatory; Stretch is optional and shows more advanced practice. A clean, well-documented Core alone is a strong result.

**Common Technical Requirements (all submissions):**
- Frontend application
- Backend API
- Database persistence
- Database setup or migration scripts
- Seed or sample data
- Input validation
- Error handling
- One working search or filter capability (Core); more in Stretch
- At least one meaningful test tier (Core); more in Stretch
- README setup instructions
- Full prompt history
- All planning, design, testing, debugging, review, reflection, and PR artifacts in the repository structure

**Database Requirement:** A database is mandatory (SQL Server chosen for this project). Provide database choice, setup instructions, a schema/migration/initialization script, seed data, an environment variable example (if applicable), and steps to run locally.

**Authentication:** Optional — counts as Stretch evidence if implemented well (login/logout, JWT or session auth, role-based access, protected routes, API authorization).

## 6. Project: Backend-Heavy — Support Ticket Management System

**Business Context:** A small application for managing support tickets. Internal users create, update, comment on, search, and progress tickets through a defined lifecycle.

### Core (Mandatory)

**Entities**
```
User (seeded only — no user-management UI required)
- id, name, email, role

Ticket
- id, title, description, priority, status,
  assignedTo, createdBy, createdAt, updatedAt

Comment
- id, ticketId, message, createdBy, createdAt
```

**Features**
- Create a ticket.
- List tickets.
- View ticket detail.
- Update ticket fields (title, description, priority, assignee).
- Change ticket status through the enforced state machine.
- Add comments to a ticket.
- Keyword search and filter by status.
- Persist all data; data survives restart.
- Validate required fields; reject invalid input at the backend.
- Show meaningful error states in the UI.

**Status State Machine (signature judgment piece — kept in Core)**
```
Open        -> In Progress
In Progress -> Resolved
Resolved    -> Closed
Open        -> Cancelled
In Progress -> Cancelled
```
Invalid transitions must be rejected by the backend and handled clearly in the frontend. This is deliberately the hardest part of Core because it is where engineering judgment shows.

**Mandatory test tier:** integration tests that prove the state-machine rules — valid transitions succeed, invalid transitions are rejected.

### Stretch (Optional — evidence toward C1.1)
- Third entity or richer data model
- Full user CRUD and role management
- Authentication, protected routes, API authorization checks
- Filter by priority and assignee; sorting; pagination
- Additional test tiers: unit tests and edge-case/failure tests
- API documentation (Swagger / OpenAPI)
- Docker setup, CI workflow
- Reusable prompt templates, rules, or specs (persistent project context)

### Core Acceptance Criteria
1. A user can create a ticket via the UI.
2. A user can view all tickets from the database.
3. A user can open a ticket detail view.
4. A user can update ticket fields and reassign.
5. A user can add comments.
6. Status changes only through valid transitions; invalid ones are rejected.
7. Keyword search and status filter work.
8. Data remains available after restart.
9. Backend validation prevents invalid records.
10. No secrets committed to the repo.
11. State-machine integration tests pass.

## 7. Required Repository Structure
```
ai-practical-assessment/
├── README.md
├── candidate-info.md
├── tool-workflow.md
├── requirements-analysis.md
├── acceptance-criteria.md
├── implementation-plan.md
├── design-notes.md
├── api-contract.md
├── data-model.md
├── ui-flow.md
├── test-strategy.md
├── test-results.md
├── debugging-notes.md
├── code-review-notes.md
├── review-fixes.md
├── pr-description.md
├── reflection.md
├── final-ai-usage-summary.md
├── src/
├── tests/
├── database/
│   ├── schema-or-migrations/
│   ├── seed-data/
│   └── setup-notes.md
├── ai-prompts/
│   ├── planning.md
│   ├── design.md
│   ├── implementation.md
│   ├── testing.md
│   ├── debugging.md
│   ├── code-review.md
│   └── documentation.md
└── tool-specific/
    └── cursor-workflow/
        ├── project-context.md
        ├── spec.md
        ├── tasks.md
        └── cursor-rules-or-instructions.md
```

## 8. Part C Submission Templates
Starting structures for required artifacts — a floor, not a limit.

- **candidate-info.md** — Name, Role, Primary Technology Stack, Primary AI Tool Used, Project Option Selected, Assessment Start Date, Submission Date, Project Summary, Tools Used, Setup Summary
- **requirements-analysis.md** — Selected Project Option, My Understanding (in your own words), Functional Requirements, Non-Functional Requirements, Assumptions, Clarifications, Edge Cases
- **acceptance-criteria.md** — Core, Validation, Error Handling, Testing, Documentation
- **implementation-plan.md** — Overview, Task Breakdown, Milestones, AI Usage Plan, Risks, Mitigation
- **design-notes.md** — Architecture Overview, Frontend Design, Backend Design, Database Design, Validation Strategy, Error Handling Strategy, Testing Strategy Link
- **api-contract.md** — Endpoint / Method / Path / Purpose, Request, Response, Validation Rules, Error Responses
- **test-strategy.md** — Test Scope, Unit Tests, Component Tests, API/Integration Tests, Edge Case Tests, Tests Not Covered
- **debugging-notes.md** — Issue, Problem, How I Investigated, How AI Helped, What I Validated, Final Fix
- **code-review-notes.md** — AI-Assisted Review Summary, My Review Observations, Changes Made After Review, Suggestions Rejected
- **reflection.md** — What I Built, How I Used AI, What AI Helped With Most, What AI Got Wrong, How I Validated AI Output, What I Would Improve Next, Reusable Workflow
- **pr-description.md** — Summary, Features Implemented, Technical Changes, Database Changes, Testing Done, AI Usage Summary, Screenshots/Demo Notes, Known Limitations, Future Improvements

**Prompt History Expectations:** Group by activity under `ai-prompts/`. For each prompt capture: prompt text/summary, AI response summary, what was accepted/changed/rejected, and why. Strong history shows context-setting, iteration, and validation prompts — not one-line prompts or blind copy-paste.

## 9. Tool-Specific Expectations (Cursor)
Submit `tool-specific/cursor-workflow/` with `project-context.md`, `spec.md` (this document), `tasks.md`, and `cursor-rules-or-instructions.md`. Show persistent context and traceability.

## 10. What Counts as Complete
Working frontend, working backend, database persistence, database setup/migration files, seed data, README setup instructions, basic tests, prompt history, requirement analysis, design notes, reflection, and a PR description. Missing elements redirect the growth-pointer focus toward filling those exact workflow gaps.

## 11. How to Take Part
1. Share your project through the single online submission form. No review call to book, nothing to host or deploy.
2. Provide a link to your Git repository, your chosen option, and short written answers explaining your key engineering decisions and trade-offs in your own words.
3. Keep commit history organized — some questions ask you to point to specific places in your repo (e.g., where you fixed an AI mistake).

## 12. What Good Looks Like
Feedback weighs how you used AI across the lifecycle and how well you understand and own your solution — not just whether the app runs.

- **Strong work shows:** clear problem understanding before coding, well-documented iterations, clean architecture, automated tests, honest reflections.
- **Weaker work shows:** direct copy-pasting with little understanding, shallow prompt history, broken configurations, generic documentation.

## 13. Growth Path
- Building the basics: focus on requirement gathering and context-setting.
- Solid and growing: focus on deepening test matrices and code review maturity.
- Strong across the lifecycle: focus on advanced edge-testing and reusable asset configurations.
- Ready to lead: mature workflows with reusable prompts, rules, or specs — ready to mentor others.

## 14. Summary
Complete a self-paced, one-week AI-assisted full-stack exercise using Cursor. Build the mandatory Core of the Support Ticket Management System (Backend-Heavy option); Stretch is optional.

Include: a frontend, a backend API, database persistence, setup/migration files, seed data, validation, error handling, search/filter, tests, README instructions, AI prompt/workflow history, a reflection, and a PR description. Use AI thoughtfully, make your thinking visible, and submit your repository link through the evaluation form when complete.
