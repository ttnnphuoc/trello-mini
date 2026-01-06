# Development Guide

## Table of Contents
1. [Prerequisites](#prerequisites)
2. [Project Setup](#project-setup)
3. [Development Workflow](#development-workflow)
4. [Architecture Overview](#architecture-overview)
5. [Code Standards](#code-standards)
6. [Testing](#testing)
7. [Debugging](#debugging)
8. [Performance Optimization](#performance-optimization)

## Prerequisites

### Required Software
- **.NET Core 9.0 SDK** - [Download](https://dotnet.microsoft.com/download)
- **Node.js 18+** - [Download](https://nodejs.org/)
- **Angular CLI** - Install globally: `npm install -g @angular/cli`
- **Git** - Version control
- **Visual Studio Code** (recommended) or Visual Studio

### Optional Tools
- **Postman** - API testing
- **SQL Server Management Studio** - Database management (if using SQL Server)
- **Chrome DevTools** - Frontend debugging

## Project Setup

### 1. Clone Repository
```bash
git clone https://github.com/ttnnphuoc/trello-mini.git
cd trello-mini
```

### 2. Backend Setup
```bash
cd TrelloMini.Backend/TrelloMini.Api
dotnet restore
dotnet build
dotnet run
```

The API will be available at: `https://localhost:5063` or `http://localhost:5063`

### 3. Frontend Setup
```bash
cd TrelloMini.Frontend
npm install
ng serve
```

The frontend will be available at: `http://localhost:4200`

## Development Workflow

### 1. Feature Development
```bash
# Create feature branch
git checkout -b feature/your-feature-name

# Make changes and commit
git add .
git commit -m "feat: add new feature description"

# Push and create PR
git push origin feature/your-feature-name
```

### 2. Backend Development
```bash
# Run in development mode
cd TrelloMini.Backend/TrelloMini.Api
dotnet watch run

# Run tests
dotnet test

# Generate migration (when using real database)
dotnet ef migrations add MigrationName
dotnet ef database update
```

### 3. Frontend Development
```bash
# Development server with hot reload
ng serve

# Run tests
ng test

# Run e2e tests
ng e2e

# Build for production
ng build --prod
```

## Architecture Overview

### Backend Architecture (.NET Core)

```
TrelloMini.Api/
├── Controllers/        # API endpoints
├── Models/            # Data models (Entities)
├── Data/              # Database context and configuration
├── Services/          # Business logic (future)
├── DTOs/              # Data Transfer Objects (future)
├── Middleware/        # Custom middleware (future)
└── Program.cs         # Application entry point
```

**Architecture Pattern:** MVP (Model-View-Presenter)
- **Models**: Entity Framework models (Board, List, Card)
- **Views**: API endpoints (Controllers)
- **Presenters**: Services (business logic layer)

### Frontend Architecture (Angular)

```
src/app/
├── components/        # UI components
├── services/          # HTTP services and business logic
├── models/            # TypeScript interfaces
├── guards/            # Route guards (future)
├── interceptors/      # HTTP interceptors (future)
├── shared/            # Shared components and utilities
└── app.component.ts   # Root component
```

**Architecture Pattern:** Component-Service Architecture
- **Components**: UI presentation layer
- **Services**: Data access and business logic
- **Models**: TypeScript interfaces for type safety

### Database Design

```
Board (1) ──→ (N) List (1) ──→ (N) Card

Board:
- Id (PK)
- Title
- Description
- CreatedAt
- UpdatedAt

List:
- Id (PK)
- Title
- Position
- BoardId (FK)
- CreatedAt
- UpdatedAt

Card:
- Id (PK)
- Title
- Description
- Position
- DueDate
- ListId (FK)
- CreatedAt
- UpdatedAt
```

## Code Standards

### Backend (.NET Core)

#### Naming Conventions
- **Classes**: PascalCase (`BoardController`)
- **Methods**: PascalCase (`GetBoards()`)
- **Properties**: PascalCase (`CreatedAt`)
- **Variables**: camelCase (`boardId`)
- **Constants**: PascalCase (`MaxTitleLength`)

#### Code Structure
```csharp
// Controller example
[ApiController]
[Route("api/[controller]")]
public class BoardsController : ControllerBase
{
    private readonly TrelloDbContext _context;

    public BoardsController(TrelloDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Board>>> GetBoards()
    {
        return await _context.Boards
            .Include(b => b.Lists)
            .ThenInclude(l => l.Cards)
            .ToListAsync();
    }
}
```

### Frontend (Angular/TypeScript)

#### Naming Conventions
- **Components**: kebab-case files, PascalCase classes (`board-list.component.ts`, `BoardListComponent`)
- **Services**: kebab-case files, PascalCase classes (`board.service.ts`, `BoardService`)
- **Interfaces**: PascalCase (`Board`, `List`, `Card`)
- **Variables/Methods**: camelCase (`boardId`, `createBoard()`)

#### Component Structure
```typescript
@Component({
  selector: 'app-board-list',
  templateUrl: './board-list.component.html',
  styleUrls: ['./board-list.component.scss']
})
export class BoardListComponent implements OnInit {
  boards: Board[] = [];

  constructor(private boardService: BoardService) { }

  ngOnInit(): void {
    this.loadBoards();
  }

  private loadBoards(): void {
    this.boardService.getBoards().subscribe(boards => {
      this.boards = boards;
    });
  }
}
```

## Testing

### Backend Testing
```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test
dotnet test --filter "TestMethodName"
```

### Frontend Testing
```bash
# Unit tests
ng test

# E2E tests
ng e2e

# Test with coverage
ng test --code-coverage
```

### Test Structure Example
```typescript
describe('BoardService', () => {
  let service: BoardService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [BoardService]
    });
    service = TestBed.inject(BoardService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  it('should fetch boards', () => {
    const mockBoards: Board[] = [
      { id: 1, title: 'Test Board', description: '', createdAt: new Date(), updatedAt: new Date(), lists: [] }
    ];

    service.getBoards().subscribe(boards => {
      expect(boards).toEqual(mockBoards);
    });

    const req = httpMock.expectOne('http://localhost:5063/api/boards');
    expect(req.request.method).toBe('GET');
    req.flush(mockBoards);
  });
});
```

## Debugging

### Backend Debugging
1. **Visual Studio Code**: Press F5 to start debugging
2. **Visual Studio**: Set breakpoints and press F5
3. **Browser**: Use Swagger UI at `https://localhost:5063/swagger`

### Frontend Debugging
1. **Browser DevTools**: F12 → Sources tab
2. **VS Code**: Use Angular Language Service extension
3. **Angular DevTools**: Browser extension for Angular debugging

### Common Issues

#### CORS Issues
```csharp
// Ensure CORS is properly configured in Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
```

#### Database Connection Issues
```csharp
// Check connection string in appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TrelloMiniDb;Trusted_Connection=true;"
  }
}
```

## Performance Optimization

### Backend
- Use async/await for database operations
- Implement proper indexing
- Use Include() wisely to avoid N+1 queries
- Consider implementing caching for frequently accessed data

### Frontend
- Use OnPush change detection strategy
- Implement lazy loading for routes
- Optimize bundle size with tree shaking
- Use trackBy functions in ngFor loops

### Example Optimizations
```typescript
// Use trackBy for better performance
trackByBoardId(index: number, board: Board): number {
  return board.id;
}

// OnPush change detection
@Component({
  selector: 'app-board',
  changeDetection: ChangeDetectionStrategy.OnPush,
  // ...
})
```

## Useful Commands

### Backend
```bash
# Create new controller
dotnet new mvc -n ControllerName

# Add Entity Framework migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Remove last migration
dotnet ef migrations remove
```

### Frontend
```bash
# Generate component
ng generate component components/component-name

# Generate service
ng generate service services/service-name

# Generate interface
ng generate interface models/interface-name

# Analyze bundle size
ng build --stats-json
npx webpack-bundle-analyzer dist/stats.json
```