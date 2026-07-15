# API Contract

## Base URLs

| Environment | API | Blazor client |
|-------------|-----|---------------|
| Development (HTTP) | `http://localhost:5041` | `http://localhost:5036` |
| Development (HTTPS) | `https://localhost:7090` | `https://localhost:7155` |

**Blazor config:** `src/TicketSystem.Blazor/wwwroot/appsettings.json` → `"ApiBaseUrl": "http://localhost:5041"`

**Swagger UI (Development only):** `http://localhost:5041/swagger`

**JSON serialization:** camelCase property names on all request/response bodies.

**CORS:** Policy `BlazorClient` allows origins `http://localhost:5036`, `https://localhost:7155`, `http://localhost:5041`, `https://localhost:7090`.

---

## Tickets

### GET /api/tickets

Retrieve all tickets with optional filtering. Does **not** include comments.

**Query parameters:**

| Name | Type | Required | Description |
|------|------|----------|-------------|
| `status` | string | No | Filter by status: `Open`, `InProgress`, `Resolved`, `Closed`, `Cancelled` |
| `keyword` | string | No | Case-sensitive `Contains` search on `title` and `description` |

**Response:** `200 OK`

```json
[
  {
    "id": 1,
    "title": "Printer not working",
    "description": "Office printer on floor 2 is offline",
    "priority": "Medium",
    "status": "Open",
    "assignedToId": 2,
    "assignedToName": "Support Agent",
    "createdById": 3,
    "createdByName": "Regular User",
    "createdAt": "2026-07-15T03:47:19.1505525Z",
    "updatedAt": "2026-07-15T03:47:19.1505525Z",
    "comments": []
  }
]
```

> **Note:** The `comments` property is always an empty array on the list endpoint. Use `GET /api/tickets/{id}` for comments.

**Error:** `400 Bad Request` if `status` is not a valid enum value.

```json
{
  "error": "Status must be one of: Open, InProgress, Resolved, Closed, Cancelled.",
  "errors": {
    "status": ["Status must be one of: Open, InProgress, Resolved, Closed, Cancelled."]
  },
  "requestId": "0HN..."
}
```

---

### GET /api/tickets/{id}

Retrieve a single ticket **with comments** (ordered by `createdAt` ascending).

**Path parameters:** `id` (int)

**Response:** `200 OK`

```json
{
  "id": 1,
  "title": "Printer not working",
  "description": "Office printer on floor 2 is offline",
  "priority": "Medium",
  "status": "Open",
  "assignedToId": 2,
  "assignedToName": "Support Agent",
  "createdById": 3,
  "createdByName": "Regular User",
  "createdAt": "2026-07-15T03:47:19.1505525Z",
  "updatedAt": "2026-07-15T03:47:19.1505525Z",
  "comments": [
    {
      "id": 1,
      "ticketId": 1,
      "message": "Looking into this now.",
      "createdById": 2,
      "createdByName": "Support Agent",
      "createdAt": "2026-07-15T04:00:00Z"
    }
  ]
}
```

**Error:** `404 Not Found`

```json
{
  "error": "Ticket not found.",
  "requestId": "0HN..."
}
```

---

### POST /api/tickets

Create a new ticket. Status is always set to `Open` server-side.

**Request body:**

```json
{
  "title": "Printer not working",
  "description": "Office printer on floor 2 is offline",
  "priority": "Medium",
  "assignedToId": 2,
  "createdById": 3
}
```

| Field | Rules |
|-------|-------|
| `title` | Required, max 200 chars |
| `description` | Required, max 5000 chars |
| `priority` | Required, one of: `Low`, `Medium`, `High`, `Critical` |
| `assignedToId` | Optional, must reference existing user |
| `createdById` | Required, must reference existing user |

**Response:** `201 Created` — body is `TicketResponseDto` (comments array empty). `Location` header points to `GET /api/tickets/{id}`.

**Errors:**
- `400` — Data annotation validation failures
- `400` — Invalid priority or non-existent user (`BusinessValidationException`)

---

### PUT /api/tickets/{id}

Update ticket fields. **Does not change status** — use the status endpoint.

**Path parameters:** `id` (int)

**Request body:**

```json
{
  "title": "Printer completely broken",
  "description": "Updated with more detail",
  "priority": "High",
  "assignedToId": 2
}
```

| Field | Rules |
|-------|-------|
| `title` | Required, max 200 chars |
| `description` | Required, max 5000 chars |
| `priority` | Required, valid priority enum |
| `assignedToId` | Optional, must reference existing user if provided |

**Response:** `200 OK` — updated `TicketResponseDto`

**Errors:** `404` ticket not found; `400` validation/business errors

---

### PUT /api/tickets/{id}/status

Change ticket status. **State machine enforced** via `TicketStateMachine`.

**Path parameters:** `id` (int)

**Request body:**

```json
{
  "newStatus": "InProgress"
}
```

| Field | Rules |
|-------|-------|
| `newStatus` | Required, valid status enum; transition must be allowed |

**Valid transitions:**

| From | Allowed `newStatus` values |
|------|---------------------------|
| `Open` | `InProgress`, `Cancelled` |
| `InProgress` | `Resolved`, `Cancelled` |
| `Resolved` | `Closed` |
| `Closed` | *(none — 400)* |
| `Cancelled` | *(none — 400)* |

**Response:** `200 OK` — updated `TicketResponseDto` with new `status` and `updatedAt`

**Error:** `400 Bad Request` — invalid transition

```json
{
  "error": "Invalid status transition",
  "currentStatus": "Open",
  "requestedStatus": "Resolved",
  "message": "Cannot transition from Open to Resolved. Valid transitions: InProgress, Cancelled",
  "requestId": "0HN..."
}
```

**Other errors:** `404` ticket not found; `400` invalid `newStatus` value

---

## Comments

Base route: `/api/tickets/{ticketId}/comments`

### GET /api/tickets/{ticketId}/comments

List all comments for a ticket, ordered by `createdAt` ascending.

**Response:** `200 OK`

```json
[
  {
    "id": 1,
    "ticketId": 1,
    "message": "Looking into this now.",
    "createdById": 2,
    "createdByName": "Support Agent",
    "createdAt": "2026-07-15T04:00:00Z"
  }
]
```

**Error:** `404` if ticket does not exist

---

### POST /api/tickets/{ticketId}/comments

Add a comment to a ticket.

**Request body:**

```json
{
  "message": "Password has been reset.",
  "createdById": 2
}
```

| Field | Rules |
|-------|-------|
| `message` | Required, max 2000 chars |
| `createdById` | Required, must reference existing user |

**Response:** `201 Created` — `CommentResponseDto`

**Errors:** `404` ticket not found; `400` validation/user errors

---

## Users

Read-only endpoints for populating assignment dropdowns in the UI.

### GET /api/users

**Response:** `200 OK`

```json
[
  {
    "id": 1,
    "name": "Admin User",
    "email": "admin@ticketsystem.com",
    "role": "Admin"
  },
  {
    "id": 2,
    "name": "Support Agent",
    "email": "agent@ticketsystem.com",
    "role": "Agent"
  }
]
```

---

### GET /api/users/{id}

**Response:** `200 OK` — single `UserResponseDto`

**Error:** `404 Not Found`

```json
{
  "error": "User not found.",
  "requestId": "0HN..."
}
```

---

## Error Response Format

All non-2xx responses use the `ApiError` shape, serialized as camelCase JSON by `ExceptionHandlingMiddleware` or `[ApiController]` model validation.

### Model validation (400) — `[ApiController]` + Data Annotations

```json
{
  "error": "One or more validation errors occurred.",
  "errors": {
    "title": ["The Title field is required."]
  },
  "requestId": "0HN..."
}
```

### Business validation (400) — `BusinessValidationException`

```json
{
  "error": "Priority must be one of: Low, Medium, High, Critical.",
  "errors": {
    "priority": ["Priority must be one of: Low, Medium, High, Critical."]
  },
  "requestId": "0HN..."
}
```

### Not found (404) — `NotFoundException`

```json
{
  "error": "Ticket not found.",
  "requestId": "0HN..."
}
```

### Invalid status transition (400) — `InvalidTransitionException`

```json
{
  "error": "Invalid status transition",
  "currentStatus": "Open",
  "requestedStatus": "Closed",
  "message": "Cannot transition from Open to Closed. Valid transitions: InProgress, Cancelled",
  "requestId": "0HN..."
}
```

### Internal server error (500)

```json
{
  "error": "An unexpected error occurred",
  "requestId": "0HN..."
}
```

Stack traces are never returned to clients. Unhandled exceptions are logged server-side with `requestId`.

---

## Endpoint Summary

| Method | Route | Purpose | Success |
|--------|-------|---------|---------|
| `GET` | `/api/tickets` | List/search tickets | 200 |
| `GET` | `/api/tickets/{id}` | Get ticket + comments | 200 |
| `POST` | `/api/tickets` | Create ticket | 201 |
| `PUT` | `/api/tickets/{id}` | Update ticket fields | 200 |
| `PUT` | `/api/tickets/{id}/status` | Change status (state machine) | 200 |
| `GET` | `/api/tickets/{ticketId}/comments` | List comments | 200 |
| `POST` | `/api/tickets/{ticketId}/comments` | Add comment | 201 |
| `GET` | `/api/users` | List users | 200 |
| `GET` | `/api/users/{id}` | Get user | 200 |
