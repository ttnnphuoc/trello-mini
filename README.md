# Trello Mini App

A simplified Trello-like task management application built with .NET Core Web API and Angular TypeScript.

🎉 **Phase 1 MVP: 100% Complete!**  
🚀 **Phase 2: 90% Complete!** (Authentication ✅, Drag & Drop ✅, Advanced Card Details ✅)

## Tech Stack

### Backend
- ✅ .NET Core 9.0 Web API
- ✅ Entity Framework Core with PostgreSQL
- ✅ JWT Authentication with secure password hashing
- ✅ RESTful API architecture
- ✅ CORS enabled for Angular frontend
- ✅ User management and authorization

### Frontend
- ✅ Angular 21 with TypeScript
- ✅ Angular Material for UI components
- ✅ TailwindCSS for utility-first styling
- ✅ Angular CDK for enhanced drag-and-drop functionality
- ✅ Reactive Forms with validation and error handling
- ✅ Route guards and authentication system

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

### ✅ Phase 1 MVP - COMPLETE
- ✅ Board creation and management
- ✅ List/Column management with CRUD operations
- ✅ Card management with CRUD operations
- ✅ RESTful API with full CRUD endpoints
- ✅ Angular services for API communication
- ✅ Enhanced drag and drop functionality with visual feedback
- ✅ Responsive design with Material + TailwindCSS

### ✅ Phase 2 Core Features - COMPLETE
- ✅ JWT Authentication system (registration, login, logout)
- ✅ User management and secure password hashing
- ✅ Protected routes and API authorization
- ✅ Enhanced drag & drop with animations and glow effects
- ✅ User session management and authentication state
- ✅ Route guards for security

### ✅ Phase 2 Advanced Features - 90% COMPLETE
- ✅ **Card Details Modal**: Professional-grade editor with Material UI
- ✅ **Priority System**: 5-level priority (None, Low, Medium, High, Critical) with color coding
- ✅ **Due Dates**: Date picker with urgency indicators (overdue/due soon/future)
- ✅ **Enhanced Cards**: Click-to-edit, visual priority indicators, due date badges
- ✅ **Rich Descriptions**: 2000-character limit with real-time counting
- ✅ **Form Validation**: Comprehensive error handling and user feedback
- ✅ **Responsive Design**: Mobile-friendly card editing experience

### 🔧 Phase 2 Remaining Features
- 🔧 Labels and tags system with filtering
- 🔧 Basic commenting functionality

### 📋 Future Features (Phase 3+)
- Real-time updates with SignalR
- Board sharing and collaboration
- Search and advanced filtering
- File attachments
- Email notifications

## Getting Started

### Prerequisites
- .NET Core 9.0 SDK
- Node.js 18+ and npm
- Angular CLI
- PostgreSQL (for database)

### Database Setup
1. Install PostgreSQL and ensure it's running
2. Update connection string in `appsettings.json` if needed
3. Apply migrations:
```bash
cd TrelloMini.Backend/TrelloMini.Api
dotnet ef database update
```

### Backend Setup
```bash
cd TrelloMini.Backend/TrelloMini.Api
dotnet restore
dotnet run
```
API will be available at: `http://localhost:5056`

### Frontend Setup
```bash
cd TrelloMini.Frontend
npm install
ng serve --port 4201
```
Frontend will be available at: `http://localhost:4201`

### First Time Setup
1. Navigate to `http://localhost:4201`
2. Click "Sign Up" to create a new account
3. Login with your credentials
4. Start creating boards and managing tasks!

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login user
- `GET /api/auth/me` - Get current user (requires JWT token)

### Boards
- `GET /api/boards` - Get all boards (protected)
- `GET /api/boards/{id}` - Get board by ID (protected)
- `POST /api/boards` - Create new board (protected)
- `PUT /api/boards/{id}` - Update board (protected)
- `DELETE /api/boards/{id}` - Delete board (protected)

### Lists
- `GET /api/lists` - Get all lists (protected)
- `GET /api/lists/{id}` - Get list by ID (protected)
- `GET /api/lists/board/{boardId}` - Get lists by board (protected)
- `POST /api/lists` - Create new list (protected)
- `PUT /api/lists/{id}` - Update list (protected)
- `DELETE /api/lists/{id}` - Delete list (protected)

### Cards
- `GET /api/cards` - Get all cards (protected)
- `GET /api/cards/{id}` - Get card by ID (protected)
- `GET /api/cards/list/{listId}` - Get cards by list (protected)
- `POST /api/cards` - Create new card (protected)
- `PUT /api/cards/{id}` - Update card (protected)
- `PUT /api/cards/{id}/move` - Move card between lists (protected)
- `DELETE /api/cards/{id}` - Delete card (protected)

**Note**: All endpoints except authentication require a valid JWT token in the Authorization header: `Bearer <token>`

## Recent Updates (Phase 2 Advanced Features)

### 🎯 Card Priority System
- **Backend**: Added `CardPriority` enum with 5 levels (None, Low, Medium, High, Critical)
- **Database**: Applied migration to add Priority field to Cards table
- **Frontend**: Color-coded priority indicators with Material UI components
- **API**: Updated card endpoints to handle priority field in create/update operations

### 📅 Enhanced Due Date Management
- **Backend**: Existing due date support enhanced with proper handling
- **Frontend**: Angular Material date picker with form validation
- **UX**: Due date urgency indicators (red=overdue, orange=due soon, gray=future)
- **Display**: Compact date format (`MMM d`) in card previews

### 🎨 Professional Card Details Modal
- **Technology**: Standalone Angular component with Material UI
- **Features**: Full CRUD operations, real-time validation, responsive design
- **Form Handling**: Reactive forms with character limits and error messages
- **User Experience**: Click-to-edit cards, drag handle preservation, mobile-optimized

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