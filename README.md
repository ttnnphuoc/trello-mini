# Trello Mini App

A simplified Trello-like task management application built with .NET Core Web API and Angular TypeScript.

🎉 **Phase 1 MVP: 100% Complete!**  
🚀 **Phase 2: 100% Complete!** (Authentication ✅, Drag & Drop ✅, Advanced Card Details ✅)  
🔥 **Phase 3: 100% Complete!** (Real-time Collaboration ✅, SignalR ✅, Board Sharing ✅)

## Tech Stack

### Backend
- ✅ .NET Core 9.0 Web API
- ✅ Entity Framework Core with PostgreSQL
- ✅ JWT Authentication with secure password hashing
- ✅ SignalR for real-time WebSocket communication
- ✅ RESTful API architecture
- ✅ CORS enabled for Angular frontend
- ✅ User management and authorization
- ✅ Role-based permissions and board collaboration

### Frontend
- ✅ Angular 21 with TypeScript
- ✅ Angular Material for UI components
- ✅ TailwindCSS for utility-first styling
- ✅ Angular CDK for enhanced drag-and-drop functionality
- ✅ Microsoft SignalR client for real-time features
- ✅ Reactive Forms with validation and error handling
- ✅ Route guards and authentication system
- ✅ Real-time collaboration with live user presence

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

### ✅ Phase 2 Advanced Features - COMPLETE
- ✅ **Card Details Modal**: Professional-grade editor with Material UI
- ✅ **Priority System**: 5-level priority (None, Low, Medium, High, Critical) with color coding
- ✅ **Due Dates**: Date picker with urgency indicators (overdue/due soon/future)
- ✅ **Enhanced Cards**: Click-to-edit, visual priority indicators, due date badges
- ✅ **Rich Descriptions**: 2000-character limit with real-time counting
- ✅ **Form Validation**: Comprehensive error handling and user feedback
- ✅ **Responsive Design**: Mobile-friendly card editing experience

### ✅ Phase 3 Real-Time Collaboration - COMPLETE
- ✅ **SignalR Integration**: WebSocket-based real-time communication
- ✅ **Live Board Updates**: Instant synchronization of all board operations
- ✅ **User Presence**: See who's online with live user indicators
- ✅ **Real-time Card Movement**: Watch cards move between lists in real-time
- ✅ **Board Sharing**: Invitation system with email-based board access
- ✅ **Role-Based Permissions**: Owner, Admin, Member, Viewer roles with granular permissions
- ✅ **Notification System**: Real-time notifications for board activities
- ✅ **JWT + SignalR**: Secure authentication for WebSocket connections
- ✅ **Auto Reconnection**: Robust connection handling with automatic retry

### 📋 Future Features (Phase 4+)
- Labels and tags system with filtering
- Advanced search and filtering capabilities
- File attachments and media support
- Email notifications for board activities
- Board templates and archiving
- Activity timeline and audit logs

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
- `GET /api/boards` - Get user's boards with member access (protected)
- `GET /api/boards/{id}` - Get board by ID with permission check (protected)
- `POST /api/boards` - Create new board and become owner (protected)
- `PUT /api/boards/{id}` - Update board (requires edit permission)
- `DELETE /api/boards/{id}` - Delete board (owner only)

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

### Board Members (NEW in Phase 3)
- `GET /api/boardmembers/board/{boardId}` - Get board members (protected)
- `POST /api/boardmembers/{boardId}/invite` - Invite user to board (admin+ only)
- `POST /api/boardmembers/accept/{token}` - Accept board invitation
- `PUT /api/boardmembers/{boardId}/{userId}/role` - Update member role (admin+ only)
- `DELETE /api/boardmembers/{boardId}/{userId}` - Remove member (admin+ or self)

### Notifications (NEW in Phase 3)
- `GET /api/notifications` - Get user notifications with pagination (protected)
- `GET /api/notifications/unread-count` - Get unread notification count (protected)
- `PUT /api/notifications/{id}/read` - Mark notification as read (protected)
- `PUT /api/notifications/mark-all-read` - Mark all notifications as read (protected)
- `DELETE /api/notifications/{id}` - Delete notification (protected)

### SignalR Hub (NEW in Phase 3)
- **WebSocket Endpoint**: `/boardHub` - Real-time board collaboration
- **Authentication**: JWT token via query parameter (`?access_token=<jwt>`)
- **Events**: `UserJoined`, `UserLeft`, `CardMoved`, `CardUpdated`, `CardCreated`, `CardDeleted`, `ListCreated`, `ListDeleted`

**Note**: All endpoints except authentication require a valid JWT token in the Authorization header: `Bearer <token>`

## Recent Updates

### 🔥 Phase 3 Real-Time Collaboration Implementation (LATEST)

#### 🔧 Backend Architecture
- **SignalR Hub**: Complete real-time WebSocket communication system
- **Database Models**: 
  - `BoardMember` with role-based permissions (Owner/Admin/Member/Viewer)
  - `BoardInvitation` for email-based board sharing
  - `Notification` system for activity tracking
- **Security**: JWT authentication integrated with SignalR connections
- **API Controllers**: 
  - `BoardMembersController` for invitation management
  - `NotificationsController` for real-time notifications
  - Enhanced `BoardsController` with permission checks

#### 🎨 Frontend Real-Time Features
- **SignalR Service**: Angular service with automatic reconnection and connection state management
- **Live Updates**: Instant synchronization of all board operations across users
- **User Presence**: Real-time display of online users with avatars and status indicators
- **Event Handling**: Comprehensive real-time event system for cards, lists, and board changes
- **Observable Patterns**: Reactive programming with RxJS for seamless UI updates

#### 🛡️ Permission System
- **Owner**: Full board control including deletion and member management
- **Admin**: Manage members, edit board settings, full content access
- **Member**: Create lists, edit cards, participate in collaboration
- **Viewer**: Read-only access to board content

#### 🔗 Technical Achievements
- **WebSocket Security**: JWT token authentication for SignalR connections
- **Database Migrations**: Applied collaboration schema changes with proper relationships
- **Type Safety**: Comprehensive TypeScript interfaces for all real-time events
- **Error Handling**: Robust connection management with automatic retry logic
- **Memory Management**: Proper subscription cleanup to prevent memory leaks

### 🎯 Phase 2 Advanced Features (Previously Completed)

#### Card Priority System
- **Backend**: Added `CardPriority` enum with 5 levels (None, Low, Medium, High, Critical)
- **Database**: Applied migration to add Priority field to Cards table
- **Frontend**: Color-coded priority indicators with Material UI components
- **API**: Updated card endpoints to handle priority field in create/update operations

#### Enhanced Due Date Management
- **Backend**: Existing due date support enhanced with proper handling
- **Frontend**: Angular Material date picker with form validation
- **UX**: Due date urgency indicators (red=overdue, orange=due soon, gray=future)
- **Display**: Compact date format (`MMM d`) in card previews

#### Professional Card Details Modal
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