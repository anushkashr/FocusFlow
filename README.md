# FocusFlow

A task manager built with **ASP.NET Core Web API**, **EF Core (SQLite)**, **JWT authentication** and a **Blazor WebAssembly** front end, styled with a custom "Ocean" theme.

## Run it

You need the .NET 10 SDK. Nothing else: SQLite comes inside the NuGet package.

**In VS Code:** press **Cmd+Shift+B** and it runs "Run FocusFlow (API + Client)" from `.vscode/tasks.json`.

**In a terminal** (two tabs):

```bash
dotnet run --project FocusFlow.Api
```

```bash
dotnet run --project FocusFlow.Client
```

Then open **http://localhost:5178**.

| What | Where |
|---|---|
| Website (Blazor) | http://localhost:5178 |
| API | http://localhost:5142 |
| Swagger UI | http://localhost:5142/swagger |
| Demo login | `demo` / `FocusFlow123!` (set in `FocusFlow.Api/appsettings.Development.json`) |

On first run the API creates `FocusFlow.Api/focusflow.db`, then adds the demo user and 12 sample tasks. Delete that file to start fresh.

## Requirements checklist

| # | Requirement | Where |
|---|---|---|
| 1 | CRUD API with EF Core: Title, Description, DueDate, Status | `FocusFlow.Api/Controllers/TasksController.cs`, `Services/TaskService.cs`, `Data/AppDbContext.cs` |
| 1 | Tasks shown in a Blazor page | `FocusFlow.Client/Pages/Tasks.razor` |
| 2 | Blazor form with data annotations and inline messages | `Components/TaskForm.razor` and `FocusFlow.Shared/Tasks/TaskInput.cs` (`[Required]`, custom `[FutureDate]`) |
| 3 | Reusable `TaskCard` with task data and an "on completed" callback | `Components/TaskCard.razor`, used in `Pages/Dashboard.razor` and `Pages/Tasks.razor` |
| 4 | JWT in the API | `Auth/AuthService.cs` creates the token, `Program.cs` validates it, `[Authorize]` on write endpoints |
| 4 | `AuthenticationStateProvider` in Blazor | `Auth/JwtAuthStateProvider.cs`, `Auth/TokenStore.cs` |
| 4 | Create and delete hidden from guests, API returns 401 without a token | `<AuthorizeView>` in the pages and `TaskCard`, `[Authorize]` in `TasksController` |
| 5 | Server-side paging, Next/Previous | `TaskService.GetTasksAsync` (`Skip`/`Take`), `Components/Pager.razor` |
| 5 | Bonus: filter All / Pending / Completed | `?status=` query parameter, tabs in `Pages/Tasks.razor` |

## How it's organised

```
FocusFlow.sln
├── FocusFlow.Shared     Models used by both sides: DTOs, validation rules, PagedResult
├── FocusFlow.Api        ASP.NET Core Web API
│   ├── Controllers      HTTP endpoints (AuthController, TasksController)
│   ├── Services         Business logic (TaskService)
│   ├── Auth             JWT creation and settings
│   ├── Data             AppDbContext, entities, migrations, seed data
│   └── Program.cs       Dependency injection and the request pipeline
└── FocusFlow.Client     Blazor WebAssembly
    ├── Auth             Token storage, AuthenticationStateProvider, token handler
    ├── Services         Typed API clients and toasts
    ├── Components       TaskCard, TaskForm, Modal, Pager, StatCard, Toasts
    ├── Pages            Dashboard (/), Tasks (/tasks), Login (/login)
    └── wwwroot/css      The Ocean theme (all colors are variables at the top of app.css)
```

## API endpoints

| Method | URL | Login needed |
|---|---|---|
| POST | `/api/auth/login` | No |
| GET | `/api/tasks?page=1&pageSize=5&status=Pending` | No |
| GET | `/api/tasks/summary` | No |
| GET | `/api/tasks/{id}` | No |
| POST | `/api/tasks` | **Yes** |
| PUT | `/api/tasks/{id}` | **Yes** |
| PATCH | `/api/tasks/{id}/complete` | **Yes** |
| DELETE | `/api/tasks/{id}` | **Yes** |

`FocusFlow.Api/FocusFlow.Api.http` has ready-made requests you can send from the editor.

## Design decisions

- **Shared project.** `TaskInput`'s validation rules run in the Blazor form *and* again in the API, from one class.
- **SQLite instead of in-memory.** Data survives restarts, constraints are enforced, and schema changes go through migrations. Switching to SQL Server is one line in `Program.cs`.
- **`TaskItem`, not `Task`.** Avoids clashing with `System.Threading.Tasks.Task`.
- **`DateOnly` for due dates.** A due date is a day, so there are no time zone surprises.
- **Hashed passwords.** The demo user's password is stored with ASP.NET Core's `PasswordHasher`.
- **One place adds the token.** `AuthTokenHandler` attaches `Authorization: Bearer …` to every API call. If the API answers 401, the client logs out and asks you to log in again.
- **Dev-only secrets.** The JWT key and demo password live in `appsettings.Development.json`. In production, set `Jwt__Key` as an environment variable or use user secrets.

## Database migrations

The EF Core tool is installed locally for this repo (`dotnet-tools.json`):

```bash
dotnet tool restore
dotnet ef migrations add <Name> --project FocusFlow.Api --output-dir Data/Migrations
```

The API applies pending migrations at startup.
