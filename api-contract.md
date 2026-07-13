# API Contract

## Base URL

```
Development: https://localhost:7xxx
```

## Tickets Endpoints

### GET /api/tickets

**Purpose:** Retrieve all tickets with optional filtering

**Query Parameters:**
- `status` (optional): Filter by status (Open, InProgress, Resolved, Closed, Cancelled)
- `keyword` (optional): Search in title and description

**Response:** `200 OK`
```json
[
  {
    "id": 1,
    "title": "Login button not working",
    "description": "Users cannot click the login button",
    "priority": "High",
    "status": "Open",
    "assignedToId": 2,
    "assignedToName": "John Agent",
    "createdById": 3,
    "createdByName": "Jane User",
    "createdAt": "2026-07-01T10:00:00Z",
    "updatedAt": "2026-07-01T10:00:00Z"
  }
]
```

---

### GET /api/tickets/{id}

**Purpose:** Retrieve a single ticket with comments

**Path Parameters:**
- `id`: Ticket ID (integer)

**Response:** `200 OK`
```json
{
  "id": 1,
  "title": "Login button not working",
  "description": "Users cannot click the login button",
  "priority": "High",
  "status": "Open",
  "assignedToId": 2,
  "assignedToName": "John Agent",
  "createdById": 3,
  "createdByName": "Jane User",
  "createdAt": "2026-07-01T10:00:00Z",
  "updatedAt": "2026-07-01T10:00:00Z",
  "comments": [
    {
      "id": 1,
      "message": "I will look into this",
      "createdById": 2,
      "createdByName": "John Agent",
      "createdAt": "2026-07-01T11:00:00Z"
    }
  ]
}
```

**Error Response:** `404 Not Found`
```json
{
  "error": "Ticket not found",
  "ticketId": 999
}
```

---

### POST /api/tickets

**Purpose:** Create a new ticket

**Request Body:**
```json
{
  "title": "Login button not working",
  "description": "Users cannot click the login button on the homepage",
  "priority": "High",
  "assignedToId": 2,
  "createdById": 3
}
```

**Validation Rules:**
- `title`: Required, max 200 characters
- `description`: Required, max 5000 characters
- `priority`: Required, must be one of: Low, Medium, High, Critical
- `assignedToId`: Optional, must reference existing user if provided
- `createdById`: Required, must reference existing user

**Response:** `201 Created`
```json
{
  "id": 1,
  "title": "Login button not working",
  "description": "Users cannot click the login button on the homepage",
  "priority": "High",
  "status": "Open",
  "assignedToId": 2,
  "createdById": 3,
  "createdAt": "2026-07-01T10:00:00Z",
  "updatedAt": "2026-07-01T10:00:00Z"
}
```

**Error Response:** `400 Bad Request`
```json
{
  "errors": {
    "title": ["The Title field is required."],
    "priority": ["Priority must be one of: Low, Medium, High, Critical"]
  }
}
```

---

### PUT /api/tickets/{id}

**Purpose:** Update ticket fields (not status)

**Path Parameters:**
- `id`: Ticket ID (integer)

**Request Body:**
```json
{
  "title": "Login button completely broken",
  "description": "Updated description with more details",
  "priority": "Critical",
  "assignedToId": 2
}
```

**Validation Rules:**
- Same as POST /api/tickets
- Status cannot be changed via this endpoint

**Response:** `200 OK`
```json
{
  "id": 1,
  "title": "Login button completely broken",
  "description": "Updated description with more details",
  "priority": "Critical",
  "status": "Open",
  "assignedToId": 2,
  "createdById": 3,
  "createdAt": "2026-07-01T10:00:00Z",
  "updatedAt": "2026-07-01T14:30:00Z"
}
```

**Error Response:** `404 Not Found`

---

### PUT /api/tickets/{id}/status

**Purpose:** Change ticket status (state machine enforced)

**Path Parameters:**
- `id`: Ticket ID (integer)

**Request Body:**
```json
{
  "newStatus": "InProgress"
}
```

**Validation Rules:**
- `newStatus`: Required, must be valid enum value
- Transition must be allowed by state machine

**Valid Transitions:**
- Open → InProgress, Cancelled
- InProgress → Resolved, Cancelled
- Resolved → Closed

**Response:** `200 OK`
```json
{
  "id": 1,
  "status": "InProgress",
  "updatedAt": "2026-07-01T15:00:00Z"
}
```

**Error Response:** `400 Bad Request` (Invalid transition)
```json
{
  "error": "Invalid status transition",
  "currentStatus": "Open",
  "requestedStatus": "Resolved",
  "message": "Cannot transition from Open to Resolved. Valid transitions: InProgress, Cancelled"
}
```

---

## Comments Endpoints

### POST /api/tickets/{ticketId}/comments

**Purpose:** Add a comment to a ticket

**Path Parameters:**
- `ticketId`: Ticket ID (integer)

**Request Body:**
```json
{
  "message": "I will investigate this issue",
  "createdById": 2
}
```

**Validation Rules:**
- `message`: Required, max 2000 characters
- `createdById`: Required, must reference existing user
- `ticketId`: Must reference existing ticket

**Response:** `201 Created`
```json
{
  "id": 1,
  "ticketId": 1,
  "message": "I will investigate this issue",
  "createdById": 2,
  "createdByName": "John Agent",
  "createdAt": "2026-07-01T11:00:00Z"
}
```

**Error Response:** `404 Not Found` (Ticket not found)

---

### GET /api/tickets/{ticketId}/comments

**Purpose:** Get all comments for a ticket

**Path Parameters:**
- `ticketId`: Ticket ID (integer)

**Response:** `200 OK`
```json
[
  {
    "id": 1,
    "message": "I will investigate this issue",
    "createdById": 2,
    "createdByName": "John Agent",
    "createdAt": "2026-07-01T11:00:00Z"
  },
  {
    "id": 2,
    "message": "Issue is now fixed",
    "createdById": 2,
    "createdByName": "John Agent",
    "createdAt": "2026-07-01T15:00:00Z"
  }
]
```

---

## Users Endpoints

### GET /api/users

**Purpose:** Get all users (for assignment dropdown)

**Response:** `200 OK`
```json
[
  {
    "id": 1,
    "name": "Admin User",
    "email": "admin@example.com",
    "role": "Admin"
  },
  {
    "id": 2,
    "name": "John Agent",
    "email": "agent@example.com",
    "role": "Agent"
  },
  {
    "id": 3,
    "name": "Jane User",
    "email": "user@example.com",
    "role": "User"
  }
]
```

---

### GET /api/users/{id}

**Purpose:** Get a single user

**Path Parameters:**
- `id`: User ID (integer)

**Response:** `200 OK`
```json
{
  "id": 2,
  "name": "John Agent",
  "email": "agent@example.com",
  "role": "Agent"
}
```

**Error Response:** `404 Not Found`

---

## Error Response Format

All error responses follow a consistent format:

**Validation Errors (400):**
```json
{
  "errors": {
    "fieldName": ["Error message 1", "Error message 2"]
  }
}
```

**General Errors:**
```json
{
  "error": "Error message",
  "detail": "Additional detail if available"
}
```

**Internal Server Error (500):**
```json
{
  "error": "An unexpected error occurred",
  "requestId": "correlation-id-here"
}
```
