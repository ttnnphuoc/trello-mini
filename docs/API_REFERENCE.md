# API Reference

## Base URL
```
http://localhost:5063/api
```

## Authentication
Currently, no authentication is required. JWT authentication will be added in future versions.

---

## Boards API

### Get All Boards
**GET** `/boards`

Returns a list of all boards with their associated lists and cards.

**Response:**
```json
[
  {
    "id": 1,
    "title": "My Project Board",
    "description": "Project management board",
    "createdAt": "2024-01-06T10:00:00Z",
    "updatedAt": "2024-01-06T10:00:00Z",
    "lists": [
      {
        "id": 1,
        "title": "To Do",
        "position": 0,
        "boardId": 1,
        "createdAt": "2024-01-06T10:00:00Z",
        "updatedAt": "2024-01-06T10:00:00Z",
        "cards": []
      }
    ]
  }
]
```

### Get Board by ID
**GET** `/boards/{id}`

**Parameters:**
- `id` (integer) - Board ID

**Response:**
```json
{
  "id": 1,
  "title": "My Project Board",
  "description": "Project management board",
  "createdAt": "2024-01-06T10:00:00Z",
  "updatedAt": "2024-01-06T10:00:00Z",
  "lists": []
}
```

### Create Board
**POST** `/boards`

**Request Body:**
```json
{
  "title": "New Board",
  "description": "Optional description"
}
```

**Response:**
```json
{
  "id": 2,
  "title": "New Board",
  "description": "Optional description",
  "createdAt": "2024-01-06T10:00:00Z",
  "updatedAt": "2024-01-06T10:00:00Z",
  "lists": []
}
```

### Update Board
**PUT** `/boards/{id}`

**Parameters:**
- `id` (integer) - Board ID

**Request Body:**
```json
{
  "id": 1,
  "title": "Updated Board Title",
  "description": "Updated description",
  "createdAt": "2024-01-06T10:00:00Z",
  "updatedAt": "2024-01-06T10:00:00Z"
}
```

### Delete Board
**DELETE** `/boards/{id}`

**Parameters:**
- `id` (integer) - Board ID

**Response:** `204 No Content`

---

## Lists API

### Get All Lists
**GET** `/lists`

### Get List by ID
**GET** `/lists/{id}`

### Get Lists by Board
**GET** `/lists/board/{boardId}`

**Parameters:**
- `boardId` (integer) - Board ID

**Response:**
```json
[
  {
    "id": 1,
    "title": "To Do",
    "position": 0,
    "boardId": 1,
    "createdAt": "2024-01-06T10:00:00Z",
    "updatedAt": "2024-01-06T10:00:00Z",
    "cards": []
  }
]
```

### Create List
**POST** `/lists`

**Request Body:**
```json
{
  "title": "New List",
  "position": 0,
  "boardId": 1
}
```

### Update List
**PUT** `/lists/{id}`

### Delete List
**DELETE** `/lists/{id}`

---

## Cards API

### Get All Cards
**GET** `/cards`

### Get Card by ID
**GET** `/cards/{id}`

### Get Cards by List
**GET** `/cards/list/{listId}`

**Parameters:**
- `listId` (integer) - List ID

**Response:**
```json
[
  {
    "id": 1,
    "title": "Task 1",
    "description": "Task description",
    "position": 0,
    "dueDate": null,
    "listId": 1,
    "createdAt": "2024-01-06T10:00:00Z",
    "updatedAt": "2024-01-06T10:00:00Z"
  }
]
```

### Create Card
**POST** `/cards`

**Request Body:**
```json
{
  "title": "New Task",
  "description": "Optional description",
  "position": 0,
  "dueDate": null,
  "listId": 1
}
```

### Update Card
**PUT** `/cards/{id}`

### Move Card
**PUT** `/cards/{id}/move`

**Request Body:**
```json
{
  "listId": 2,
  "position": 1
}
```

### Delete Card
**DELETE** `/cards/{id}`

---

## Error Responses

### 400 Bad Request
```json
{
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Title": ["The Title field is required."]
  }
}
```

### 404 Not Found
```json
{
  "title": "Not Found",
  "status": 404,
  "detail": "The requested resource was not found."
}
```

### 500 Internal Server Error
```json
{
  "title": "An error occurred while processing your request.",
  "status": 500
}
```

---

## Data Models

### Board
- `id` (integer) - Unique identifier
- `title` (string, required, max 100 chars) - Board title
- `description` (string, optional, max 500 chars) - Board description
- `createdAt` (datetime) - Creation timestamp
- `updatedAt` (datetime) - Last update timestamp
- `lists` (array) - Associated lists

### List
- `id` (integer) - Unique identifier
- `title` (string, required, max 100 chars) - List title
- `position` (integer) - Position in board
- `boardId` (integer) - Parent board ID
- `createdAt` (datetime) - Creation timestamp
- `updatedAt` (datetime) - Last update timestamp
- `cards` (array) - Associated cards

### Card
- `id` (integer) - Unique identifier
- `title` (string, required, max 200 chars) - Card title
- `description` (string, optional, max 1000 chars) - Card description
- `position` (integer) - Position in list
- `dueDate` (datetime, optional) - Due date
- `listId` (integer) - Parent list ID
- `createdAt` (datetime) - Creation timestamp
- `updatedAt` (datetime) - Last update timestamp