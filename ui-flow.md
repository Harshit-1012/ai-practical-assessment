# UI Flow

## User Journeys

### Journey 1: Creating a New Ticket

**Goal:** User wants to report an issue by creating a ticket

**Steps:**
1. User navigates to the application home page (ticket list)
2. User clicks "Create New Ticket" button
3. System navigates to Create Ticket page
4. User fills in the form:
   - Enters ticket title (required)
   - Enters detailed description (required)
   - Selects priority from dropdown (Low, Medium, High, Critical)
   - Optionally selects an agent to assign the ticket to
5. User clicks "Submit" or "Create Ticket" button
6. System validates the input:
   - If validation fails: Display error messages inline
   - If validation succeeds: Submit to API
7. API creates the ticket with status "Open"
8. System shows success message
9. System navigates back to ticket list or shows ticket detail
10. New ticket appears in the list

**Edge Cases:**
- User leaves required fields empty → Show validation errors
- User tries to submit while loading → Disable submit button
- API error occurs → Show error message, keep form data

---

### Journey 2: Viewing and Searching Tickets

**Goal:** User wants to find and view tickets

**Steps:**
1. User navigates to ticket list page
2. System displays all tickets in a table/card layout
3. User can perform actions:
   - **Search:** Enter keyword in search box, press Enter or click Search
   - **Filter:** Select status from dropdown (All, Open, InProgress, Resolved, Closed, Cancelled)
   - **View Details:** Click on a ticket to view full details
4. System updates the list based on search/filter criteria
5. If no tickets match: Display "No tickets found" message

**Search Behavior:**
- Search is case-insensitive
- Searches in both title and description fields
- Can be combined with status filter
- Empty search shows all tickets (respecting filter)

**UI Elements:**
- Search input box with search button
- Status filter dropdown
- Ticket cards/rows showing: ID, Title, Status badge, Priority badge, Assignee, Created date
- Click anywhere on ticket card to view details

---

### Journey 3: Viewing Ticket Details and Comments

**Goal:** User wants to see full ticket information and conversation history

**Steps:**
1. User clicks on a ticket from the list
2. System navigates to Ticket Detail page
3. System displays:
   - **Ticket Header:**
     - Ticket ID and Title
     - Status badge (color-coded)
     - Priority badge (color-coded)
     - Edit button
   - **Ticket Body:**
     - Full description
     - Assigned to: Agent name (or "Unassigned")
     - Created by: User name
     - Created at: Timestamp
     - Updated at: Timestamp
   - **Status Transitions:**
     - Buttons for valid status transitions only
     - Example: If status is "Open", show "Start Progress" and "Cancel" buttons
   - **Comments Section:**
     - List of all comments in chronological order
     - Each comment shows: Author name, message, timestamp
     - Add comment form at bottom
4. User can perform actions:
   - Change status (see Journey 4)
   - Add comment (see Journey 5)
   - Edit ticket (click Edit button)
   - Navigate back to list

**Valid Status Transition Buttons:**
- **Open:** "Start Progress" (→ InProgress), "Cancel Ticket" (→ Cancelled)
- **InProgress:** "Mark Resolved" (→ Resolved), "Cancel Ticket" (→ Cancelled)
- **Resolved:** "Close Ticket" (→ Closed)
- **Closed:** No buttons (terminal state)
- **Cancelled:** No buttons (terminal state)

---

### Journey 4: Changing Ticket Status

**Goal:** User (typically an agent) wants to progress a ticket through its lifecycle

**Steps:**
1. User is viewing ticket detail page
2. User sees status transition buttons based on current status
3. User clicks a transition button (e.g., "Start Progress")
4. System shows confirmation dialog: "Change status from Open to In Progress?"
5. User confirms
6. System calls API to change status
7. API validates transition via state machine:
   - If valid: Updates ticket status and UpdatedAt timestamp
   - If invalid: Returns 400 error (should not happen if UI correctly shows buttons)
8. System updates the UI:
   - Status badge changes color/text
   - Transition buttons update to show new valid transitions
   - UpdatedAt timestamp updates
9. System shows success message: "Status changed to In Progress"

**Error Handling:**
- If API returns 400 (invalid transition): Show error message
- If concurrent update occurred: Show error, refresh ticket data
- If network error: Show error message, allow retry

---

### Journey 5: Adding a Comment

**Goal:** User wants to add information or updates to a ticket

**Steps:**
1. User is viewing ticket detail page
2. User scrolls to Comments section at bottom
3. User sees "Add Comment" form
4. User enters comment text in textarea
5. User clicks "Add Comment" button
6. System validates:
   - Comment text is not empty
   - If invalid: Show inline error
7. System submits comment to API
8. API saves comment with author and timestamp
9. System adds new comment to the comment list immediately
10. System clears the comment form
11. System shows success message (optional)

**UI Behavior:**
- Disable submit button while loading
- Clear form after successful submission
- Keep form data if submission fails
- Display new comment without page refresh

---

### Journey 6: Editing a Ticket

**Goal:** User wants to update ticket details (not status)

**Steps:**
1. User is viewing ticket detail page
2. User clicks "Edit" button
3. System navigates to Edit Ticket page
4. System pre-fills form with current ticket data:
   - Title
   - Description
   - Priority
   - Assigned to
5. User modifies one or more fields
6. User clicks "Save" or "Update Ticket" button
7. System validates input (same rules as create)
8. If valid: System submits to API
9. API updates ticket (except status) and UpdatedAt timestamp
10. System shows success message
11. System navigates back to ticket detail page
12. Updated information is displayed

**Notes:**
- Status cannot be changed via edit form (only via transition buttons on detail page)
- User can change assignee to "Unassigned" by clearing selection
- Cancel button returns to detail page without saving

---

### Journey 7: Filtering Tickets by Status

**Goal:** User wants to see only tickets in a specific status

**Steps:**
1. User is on ticket list page
2. User clicks status filter dropdown
3. Dropdown shows options: All, Open, InProgress, Resolved, Closed, Cancelled
4. User selects a status (e.g., "Open")
5. System calls API with status filter parameter
6. System updates ticket list to show only Open tickets
7. Filter dropdown shows selected status
8. User can select "All" to remove filter

**Combining with Search:**
- Search and filter work together
- Example: Search "login" + Filter "Open" = Open tickets containing "login"
- Clearing search maintains filter
- Clearing filter maintains search

---

## Page Layouts

### Ticket List Page
```
┌─────────────────────────────────────────────┐
│  [Search: _________] [🔍] [Status: All ▼]  │
│  [+ Create New Ticket]                      │
├─────────────────────────────────────────────┤
│  ┌───────────────────────────────────────┐ │
│  │ #1 Login Button Not Working           │ │
│  │ [Open] [High] Agent: John | 2h ago    │ │
│  └───────────────────────────────────────┘ │
│  ┌───────────────────────────────────────┐ │
│  │ #2 Dashboard Loading Slow             │ │
│  │ [InProgress] [Medium] Agent: Sarah    │ │
│  └───────────────────────────────────────┘ │
└─────────────────────────────────────────────┘
```

### Ticket Detail Page
```
┌─────────────────────────────────────────────┐
│  [← Back to List]                  [Edit]   │
├─────────────────────────────────────────────┤
│  Ticket #1: Login Button Not Working       │
│  [Open] [High Priority]                     │
├─────────────────────────────────────────────┤
│  Description:                               │
│  Users cannot click the login button...    │
│                                             │
│  Assigned to: John Agent                    │
│  Created by: Jane User                      │
│  Created: 2026-07-01 10:00 AM               │
│  Updated: 2026-07-01 10:00 AM               │
├─────────────────────────────────────────────┤
│  Status Actions:                            │
│  [Start Progress] [Cancel Ticket]           │
├─────────────────────────────────────────────┤
│  Comments (2)                               │
│  ┌─────────────────────────────────────┐   │
│  │ John Agent - 11:00 AM               │   │
│  │ I will investigate this issue       │   │
│  └─────────────────────────────────────┘   │
│  ┌─────────────────────────────────────┐   │
│  │ John Agent - 2:00 PM                │   │
│  │ Found the bug, working on fix       │   │
│  └─────────────────────────────────────┘   │
│                                             │
│  Add Comment:                               │
│  [___________________________________]      │
│  [Add Comment]                              │
└─────────────────────────────────────────────┘
```

### Create/Edit Ticket Page
```
┌─────────────────────────────────────────────┐
│  [← Back]                                   │
├─────────────────────────────────────────────┤
│  Create New Ticket                          │
├─────────────────────────────────────────────┤
│  Title *                                    │
│  [_____________________________________]    │
│                                             │
│  Description *                              │
│  [_____________________________________]    │
│  [_____________________________________]    │
│  [_____________________________________]    │
│                                             │
│  Priority *                                 │
│  [Medium ▼]                                 │
│                                             │
│  Assign To                                  │
│  [-- Select Agent -- ▼]                    │
│                                             │
│  [Cancel] [Create Ticket]                   │
└─────────────────────────────────────────────┘
```

---

## Navigation Flow

```
Ticket List
    ├─→ Create Ticket → [Submit] → Ticket List
    ├─→ View Ticket Detail
    │   ├─→ Edit Ticket → [Save] → Ticket Detail
    │   ├─→ Change Status → [Confirm] → Ticket Detail (updated)
    │   ├─→ Add Comment → [Submit] → Ticket Detail (updated)
    │   └─→ Back → Ticket List
    └─→ Search/Filter → Ticket List (filtered)
```
