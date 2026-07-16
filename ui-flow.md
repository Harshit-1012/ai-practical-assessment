# UI Flow

This document describes the **implemented** Blazor WebAssembly UI (`TicketSystem.Blazor`). Routes, layouts, labels, and interaction patterns match the actual pages and components in the codebase.

---

## Application Shell

All ticket pages render inside `MainLayout`:

- **Left sidebar** (`NavMenu`): links to **All Tickets** (`/tickets`) and **New Ticket** (`/tickets/create`)
- **Top bar**: "TicketDesk" branding with "Support Ticket Management" tagline
- **Main content area**: page body
- **Toast container** (`ToastContainer`): global success/error notifications via `NotificationService`

**Entry route:** `/` (`Index.razor`) immediately redirects to `/tickets`.

Styling uses a custom indigo/slate theme in `wwwroot/css/app.css` (not Bootstrap).

---

## User Journeys

### Journey 1: Creating a New Ticket

**Goal:** Report a new support issue.

**Entry points:**
- Ticket list header: **+ New Ticket**
- Sidebar: **New Ticket**
- Empty state on ticket list: **Create Ticket**

**Steps:**
1. User navigates to `/tickets/create`
2. Page shows "Create Ticket" with subtitle "Report a new support issue"
3. While users load from the API, a spinner displays ("Loading form...")
4. User completes the form:
   - **Title** (required, max 200 characters)
   - **Description** (required, max 5000 characters)
   - **Priority** (required dropdown: Low, Medium, High, Critical; defaults to **Medium**)
   - **Assign To** (optional; "Unassigned" or a user shown as `Name (Role)`)
   - **Created By** (required dropdown of all users; defaults to user id **3** in the model)
5. User clicks **Create Ticket** (shows "Creating..." while submitting; button disabled during submit)
6. Client-side validation runs via `DataAnnotationsValidator`; failures show inline `ValidationMessage` text
7. On success:
   - Toast: "Ticket created successfully."
   - Navigation to **ticket detail** (`/tickets/{id}`) — not back to the list
8. On API error: inline alert plus error toast; form data retained

**Cancel:** **Cancel** button navigates to `/tickets` without saving.

---

### Journey 2: Viewing, Searching, and Filtering Tickets

**Goal:** Browse tickets and narrow results.

**Route:** `/tickets`

**Steps:**
1. Page loads with header "Support Tickets" and subtitle "Track, search, and manage support requests"
2. While tickets load, `LoadingSpinner` shows "Loading tickets..."
3. Tickets render in a **responsive card grid** (not a table). Each card is clickable and navigates to `/tickets/{id}`.
4. Each card shows:
   - Ticket id (`#123`)
   - Status and priority badges
   - Title
   - Description preview (clamped to **2 lines**)
   - Footer: assignee (or "Unassigned") and **relative** created date (e.g. "2h ago", "Just now")
5. **Search** (`SearchFilter` component):
   - Keyword input with placeholder "Search title or description..."
   - **Search** button applies the filter
   - **Enter** in the keyword field also applies the filter
   - Search is case-insensitive on title and description (API behavior)
6. **Status filter** dropdown:
   - Options: All Statuses, Open, In Progress, Resolved, Closed, Cancelled
   - Changing the dropdown **immediately** re-queries the API (no separate Search click required)
7. Search keyword and status filter can be combined
8. If no tickets match: `EmptyState` with "No tickets found", guidance text, and a **Create Ticket** button
9. On load failure: inline error alert and error toast (including a friendly message when the API is unreachable)

**Note:** There is no pagination; the list shows all results returned by the API for the current criteria.

---

### Journey 3: Viewing Ticket Details and Comments

**Goal:** See full ticket metadata, change status, and read/post comments.

**Route:** `/tickets/{TicketId}`

**Steps:**
1. User opens a ticket from the list (or lands here after create)
2. While loading, spinner shows "Loading ticket..."
3. Page header:
   - **← Back to tickets** link → `/tickets`
   - Ticket **title** as `h1`
   - Subtitle: `Ticket #{id}`
   - **Edit** button (top right) → `/tickets/{id}/edit`
4. **Two-column layout:**
   - **Main card (left):**
     - Status and priority badges
     - Metadata grid: Assigned to, Created by, Created (absolute datetime), Updated (absolute datetime)
     - Description section with full text
   - **Sidebar (right):**
     - `StatusTransitionActions` panel titled **Change Status** (see Journey 4)
5. **Comments section** (full width below):
   - Heading: `Comments ({count})`
   - `CommentList`: chronological comment cards (author name, absolute timestamp, message)
   - Empty comments: "No comments yet. Be the first to add an update."
   - `AddComment` form below the list (see Journey 5)

**Load errors:** API failures show an error toast only; the page body stays empty after the spinner (no inline error panel on this page).

---

### Journey 4: Changing Ticket Status

**Goal:** Move a ticket through its lifecycle.

**There is no confirmation dialog.** A single click on an enabled transition button calls the API immediately.

**Steps:**
1. User is on ticket detail; the sidebar shows **Change Status**
2. The UI renders a button for **every status except the current one** (four buttons when not terminal):
   - **Start Progress** → InProgress
   - **Mark Resolved** → Resolved
   - **Close Ticket** → Closed
   - **Cancel Ticket** → Cancelled
3. Buttons allowed by the state machine are **enabled**; all others are **disabled** with a `title` tooltip explaining why (e.g. "Cannot move from Open to Resolved. Allowed: InProgress, Cancelled.")
4. User clicks an **enabled** button
5. All transition buttons disable while the request is in flight (`isChangingStatus`)
6. API validates via the backend state machine
7. On success:
   - Ticket data refreshes in place (status badge, metadata, transition buttons update)
   - Toast: "Status changed to {label}." (e.g. "In Progress")
8. On failure: error toast; ticket state unchanged

**Valid transitions (enabled buttons by current status):**

| Current Status | Enabled Buttons |
|----------------|-----------------|
| Open | Start Progress, Cancel Ticket |
| InProgress | Mark Resolved, Cancel Ticket |
| Resolved | Close Ticket |
| Closed | *(all four buttons disabled — terminal)* |
| Cancelled | *(all four buttons disabled — terminal)* |

Invalid targets are still visible but disabled; the UI does not hide them.

---

### Journey 5: Adding a Comment

**Goal:** Post an update on a ticket.

**Steps:**
1. User scrolls to the comments section on ticket detail
2. Form label: **Add a comment**
3. User enters text in a textarea (required, max 2000 characters per API validation)
4. User clicks **Post Comment** (shows "Posting..." while submitting)
5. Client validation via `DataAnnotationsValidator`; failures show inline messages
6. Comment is submitted with `CreatedById` fixed to **user id 3** (`AddComment` `DefaultUserId`; no author picker in the UI)
7. On success:
   - Toast: "Comment added."
   - Textarea cleared
   - Parent page **reloads the full ticket** (`ReloadTicketAsync`) so the new comment appears in the list
8. On failure: inline alert plus error toast; message text retained

---

### Journey 6: Editing a Ticket

**Goal:** Update title, description, priority, or assignee (not status).

**Route:** `/tickets/{TicketId}/edit`

**Steps:**
1. User clicks **Edit** on ticket detail
2. While loading, spinner shows "Loading ticket..."
3. Page header: "Edit Ticket #{id}" with ticket title as subtitle
4. Form pre-filled with current title, description, priority, and assignee
5. User edits fields (same validation rules as create for title/description/priority)
6. **Assign To**: "Unassigned" or `Name (Role)` — there is no **Created By** field on edit
7. User clicks **Save Changes** (shows "Saving..." while submitting)
8. On success:
   - Toast: "Ticket updated."
   - Navigation back to ticket detail
9. On failure: inline alert plus error toast

**Cancel:** **Cancel** navigates to `/tickets/{TicketId}` without saving.

**Notes:**
- Status cannot be changed on this form; use **Change Status** on the detail page
- The **Edit** button on detail is always shown (including for Closed/Cancelled tickets); the API may reject updates to terminal tickets

---

## Page Layouts

### Application Shell
```
┌──────────────┬──────────────────────────────────────────────┐
│  Navigation  │  TS  TicketDesk                              │
│              │      Support Ticket Management               │
│  📋 All      ├──────────────────────────────────────────────┤
│     Tickets  │                                              │
│              │           (page content)                     │
│  ＋ New      │                                              │
│     Ticket   │                              [toast alerts]  │
└──────────────┴──────────────────────────────────────────────┘
```

### Ticket List (`/tickets`)
```
┌─────────────────────────────────────────────────────────────┐
│  Support Tickets                          [+ New Ticket]    │
│  Track, search, and manage support requests                 │
├─────────────────────────────────────────────────────────────┤
│  Search [________________________] [Search]                 │
│  Status [All Statuses ▼]   ← changing status re-queries     │
├─────────────────────────────────────────────────────────────┤
│  ┌─────────────────────┐  ┌─────────────────────┐           │
│  │ #1    [Open][High]  │  │ #2 [InProgress][Med]│           │
│  │ Login Button Not... │  │ Dashboard Loading...│           │
│  │ Users cannot click..│  │ Page takes too long │  (2-line  │
│  │ Unassigned  2h ago  │  │ John Agent    1d ago│   clamp)  │
│  └─────────────────────┘  └─────────────────────┘           │
└─────────────────────────────────────────────────────────────┘
```

### Ticket Detail (`/tickets/{id}`)
```
┌─────────────────────────────────────────────────────────────┐
│  [← Back to tickets]                              [Edit]    │
│  Login Button Not Working                                   │
│  Ticket #1                                                  │
├──────────────────────────────┬──────────────────────────────┤
│  [Open] [High]               │  Change Status               │
│                              │  [Start Progress]            │
│  Assigned to   Unassigned    │  [Mark Resolved]  (disabled) │
│  Created by    Jane User     │  [Close Ticket]   (disabled) │
│  Created       Jul 1, 10:00  │  [Cancel Ticket]             │
│  Updated       Jul 1, 10:00  │                              │
│                              │  (invalid buttons disabled   │
│  Description                 │   with hover tooltip)        │
│  Users cannot click the...   │                              │
├──────────────────────────────┴──────────────────────────────┤
│  Comments (2)                                               │
│  ┌─────────────────────────────────────────────────────┐   │
│  │ John Agent                    Jul 1, 2026 11:00 AM   │   │
│  │ I will investigate this issue                        │   │
│  └─────────────────────────────────────────────────────┘   │
│  Add a comment                                              │
│  [____________________________________________]             │
│  [Post Comment]                                             │
└─────────────────────────────────────────────────────────────┘
```

### Create Ticket (`/tickets/create`)
```
┌─────────────────────────────────────────────────────────────┐
│  Create Ticket                                              │
│  Report a new support issue                                 │
├─────────────────────────────────────────────────────────────┤
│  Title *                                                    │
│  [_____________________________________]                    │
│  Description *                                              │
│  [_____________________________________]  (5 rows)          │
│  Priority *          Assign To                              │
│  [Medium ▼]          [Unassigned ▼]                           │
│  Created By *                                               │
│  [Jane User ▼]                                              │
│                                                             │
│  [Cancel]  [Create Ticket]                                  │
└─────────────────────────────────────────────────────────────┘
```

### Edit Ticket (`/tickets/{id}/edit`)
```
┌─────────────────────────────────────────────────────────────┐
│  Edit Ticket #1                                             │
│  Login Button Not Working                                   │
├─────────────────────────────────────────────────────────────┤
│  Title *                                                    │
│  [_____________________________________]                    │
│  Description *                                              │
│  [_____________________________________]                    │
│  Priority *          Assign To                              │
│  [High ▼]            [John Agent (Agent) ▼]               │
│                                                             │
│  [Cancel]  [Save Changes]                                   │
└─────────────────────────────────────────────────────────────┘
```

---

## Navigation Flow

```
/  →  redirect  →  /tickets

Sidebar: All Tickets (/tickets)  |  New Ticket (/tickets/create)

/tickets
    ├─→ + New Ticket / sidebar New Ticket  →  /tickets/create
    │       └─→ [Create Ticket]  →  /tickets/{id}  (detail)
    ├─→ click ticket card  →  /tickets/{id}
    └─→ Search / Status filter  →  /tickets  (same page, refreshed list)

/tickets/{id}
    ├─→ ← Back to tickets  →  /tickets
    ├─→ Edit  →  /tickets/{id}/edit
    │       └─→ [Save Changes]  →  /tickets/{id}
    │       └─→ [Cancel]  →  /tickets/{id}
    ├─→ [enabled status button]  →  /tickets/{id}  (in-place update, no dialog)
    └─→ [Post Comment]  →  /tickets/{id}  (reload ticket data)
```

---

## Shared UI Patterns

| Pattern | Implementation |
|---------|----------------|
| Loading | `LoadingSpinner` with context message |
| Empty results | `EmptyState` on ticket list; compact empty text in `CommentList` |
| Success feedback | Toast via `NotificationService` |
| Error feedback | Toast + inline `alert alert-error` on forms and list load |
| Status display | `StatusBadge` with color-coded classes |
| Priority display | `PriorityBadge` with color-coded classes |
| Date formatting | Absolute (`MMM d, yyyy h:mm tt`) on detail; relative on list cards |

---

## Key Components

| Component | Role |
|-----------|------|
| `SearchFilter` | Keyword + status filter; emits `TicketSearchCriteria` to parent |
| `StatusTransitionActions` | Renders all non-current status buttons; disables invalid ones |
| `TicketWorkflowService` | UI mirror of backend state machine (labels, enabled/disabled, tooltips) |
| `CommentList` | Renders comment cards or empty message |
| `AddComment` | Comment form; posts with fixed `CreatedById` |
| `TicketApiService` | HTTP calls for ticket CRUD, search, status change |
| `CommentApiService` | HTTP call to add comments |
| `UserApiService` | Loads users for create/edit assignee and create author dropdowns |
