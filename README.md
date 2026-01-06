# Trello Mini App

A simplified Trello-like task management application built with .NET Core Web API and Angular TypeScript.

## Tech Stack

### Backend
- .NET Core 9.0 Web API
- Entity Framework Core (In-Memory Database)
- RESTful API architecture
- CORS enabled for Angular frontend

### Frontend
- Angular 21 with TypeScript
- Angular Material for UI components
- TailwindCSS for utility-first styling
- Angular CDK for drag-and-drop functionality
- Reactive Forms for form handling

## Project Structure

```
TrelloMini/
├── TrelloMini.Backend/
│   └── TrelloMini.Api/
│       ├── Controllers/     # API Controllers
│       ├── Models/         # Data Models
│       ├── Data/           # DbContext
│       └── Services/       # Business Logic
└── TrelloMini.Frontend/
    └── src/
        ├── app/
        │   ├── models/     # TypeScript Interfaces
        │   ├── services/   # HTTP Services
        │   └── components/ # Angular Components
        └── styles.scss     # Global Styles
```

## Features

### MVP (Minimum Viable Product)
- ✅ Board creation and management
- ✅ List/Column management with CRUD operations
- ✅ Card management with CRUD operations
- ✅ RESTful API with full CRUD endpoints
- ✅ Angular services for API communication
- 🔄 Drag and drop functionality (In Progress)
- 🔄 Responsive design with Material + Tailwind (In Progress)

### Planned Features
- Authentication with JWT tokens
- Real-time updates
- Card due dates and descriptions
- Search and filter functionality
- User management
- Board sharing capabilities

## Getting Started

### Prerequisites
- .NET Core 9.0 SDK
- Node.js 18+ and npm
- Angular CLI

### Backend Setup
```bash
cd TrelloMini.Backend/TrelloMini.Api
dotnet restore
dotnet run
```
API will be available at: `http://localhost:5063`

### Frontend Setup
```bash
cd TrelloMini.Frontend
npm install
ng serve
```
Frontend will be available at: `http://localhost:4200`

## API Endpoints

### Boards
- `GET /api/boards` - Get all boards
- `GET /api/boards/{id}` - Get board by ID
- `POST /api/boards` - Create new board
- `PUT /api/boards/{id}` - Update board
- `DELETE /api/boards/{id}` - Delete board

### Lists
- `GET /api/lists` - Get all lists
- `GET /api/lists/{id}` - Get list by ID
- `GET /api/lists/board/{boardId}` - Get lists by board
- `POST /api/lists` - Create new list
- `PUT /api/lists/{id}` - Update list
- `DELETE /api/lists/{id}` - Delete list

### Cards
- `GET /api/cards` - Get all cards
- `GET /api/cards/{id}` - Get card by ID
- `GET /api/cards/list/{listId}` - Get cards by list
- `POST /api/cards` - Create new card
- `PUT /api/cards/{id}` - Update card
- `PUT /api/cards/{id}/move` - Move card between lists
- `DELETE /api/cards/{id}` - Delete card

## Development

This project follows MVP (Model-View-Presenter) architecture:
- **Models**: Data entities (Board, List, Card)
- **Views**: Angular components and templates
- **Presenters**: Angular services and API controllers

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## License

This project is licensed under the MIT License.