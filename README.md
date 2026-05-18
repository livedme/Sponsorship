# Sponsorship Request Approval Workflow

A multi-role enterprise workflow application for managing sponsorship requests through a structured approval pipeline.

---

## Technology Summary

### Platform & Language
- **.NET 10** — target framework across all projects
- **C#** — primary language

### Backend (API)
- **ASP.NET Core 10** — Web API framework
- **JWT Authentication** (`Microsoft.AspNetCore.Authentication.JwtBearer`) — token-based auth
- **Swagger / Swashbuckle** (v9) — API documentation & testing UI

### Frontend (UI)
- **Blazor Server** — server-side rendered SPA
- **MudBlazor** (v9.4) — Material Design UI component library

### Data Access
- **Entity Framework Core 10** — ORM
- **Npgsql EF Core Provider** — PostgreSQL driver for EF Core
- **ASP.NET Core Identity** — user/role management

### Database
- **PostgreSQL 18** — primary relational database

### Security
- **JWT tokens** (`System.IdentityModel.Tokens.Jwt`) — authentication
- **Custom `PasswordHasher`** + `System.Security.Cryptography.Xml`
- **RBAC** — 4 roles: Requestor, Manager, FinanceAdmin, SystemAdmin

### Architecture Pattern
- **Clean Architecture** — Domain / Application / Infrastructure / API layers
- **Unit of Work + Repository pattern**
- **Dependency Injection** (`Microsoft.Extensions.DependencyInjection`)

### Containerization & Deployment
- **Docker** — `Dockerfile` + `docker/Dockerfile.ui` for API and UI
- **Render.com** — hosting platform (`render.yaml`)

---

## Architecture

```
┌─────────────────────┐        HTTP/REST        ┌────────────────────────┐
│  Sponsorship.BlazorUI│ ───────────────────────▶│  Sponsorship.API       │
│  (Blazor Server)    │                          │  (ASP.NET Core 10)      │
│  Port: 7001         │                          │  Port: 5000            │
└─────────────────────┘                          └──────────┬─────────────┘
                                                             │
                                               ┌─────────────▼───────────┐
                                               │  Sponsorship.Infrastructure│
                                               │  (EF Core + Npgsql)     │
                                               └─────────────┬───────────┘
                                                             │
                                               ┌─────────────▼───────────┐
                                               │  PostgreSQL Database     │
                                               └─────────────────────────┘
```

### Projects

| Project | Description |
|---|---|
| `Sponsorship.Domain` | Entities (`SponsorshipRequest`, `User`, `WorkflowHistory`, `SponsorshipType`) and enums |
| `Sponsorship.Application` | DTOs, service interfaces, `JwtSettings` |
| `Sponsorship.Infrastructure` | EF Core `AppDbContext`, service implementations, `DataSeeder`, `PasswordHasher` |
| `Sponsorship.API` | ASP.NET Core Web API with JWT auth, Swagger |
| `Sponsorship.BlazorUI` | Blazor Server frontend with MudBlazor |

---

## Workflow

```
Draft ──▶ PendingManagerApproval ──▶ PendingFinanceReview ──▶ Approved
  │               │                           │
  └── Cancel      └── Reject                  └── Reject
```

1. **Requestor** creates a draft and submits it
2. **Manager** reviews and approves or rejects
3. **FinanceAdmin** reviews approved requests and gives final approval or rejection
4. **SystemAdmin** has read-only visibility into all requests and the full audit log

---

## RBAC

| Role | Capabilities |
|---|---|
| `Requestor` | Create, edit drafts, submit, cancel own requests |
| `Manager` | Approve / reject requests in `PendingManagerApproval` state |
| `FinanceAdmin` | Approve / reject requests in `PendingFinanceReview` state |
| `SystemAdmin` | View all requests, full audit log, manage sponsorship types |

---
## Test url
- Web Api: https://sponsorship-cgh6.onrender.com/swagger/index.html
- Frontend UI: https://www.avancerapp.com

## Test Accounts

| Email | Password | Role |
|---|---|---|
| `requestor@test.com` | `Test@1234` | Requestor |
| `manager@test.com` | `Test@1234` | Manager |
| `finance@test.com` | `Test@1234` | FinanceAdmin |
| `admin@test.com` | `Test@1234` | SystemAdmin |

---

## Local Setup

### Prerequisites

- .NET 10 SDK
- PostgreSQL 18
- `dotnet-ef` global tool: `dotnet tool install -g dotnet-ef`

### 1. Configure the database

Update `src/Sponsorship.API/appsettings.json`:

```json
"ConnectionStrings": {
  "Default": "Host=localhost;Port=5432;Database=SponsorshipDb;Username=postgres;Password=<your-password>"
}
```

### 2. Run migrations & seed data

```bash
dotnet ef database update --project src/Sponsorship.Infrastructure --startup-project src/Sponsorship.API
```

The app auto-seeds sponsorship types and test users on first start.

### 3. Start the API

```bash
cd src/Sponsorship.API
dotnet run
# API: http://localhost:5000
# Swagger: http://localhost:5000/swagger
```

### 4. Start the Blazor UI

Update `src/Sponsorship.BlazorUI/appsettings.json` if the API runs on a different port:

```json
"ApiSettings": {
  "BaseUrl": "http://localhost:5000"
}
```

```bash
cd src/Sponsorship.BlazorUI
dotnet run
# UI: http://localhost:7001
```

---

## Environment Variables (Production)

| Variable | Description |
|---|---|
| `ConnectionStrings__Default` | PostgreSQL connection string |
| `Jwt__Key` | JWT signing key (≥32 chars) |
| `Jwt__Issuer` | JWT issuer |
| `Jwt__Audience` | JWT audience |
| `ApiSettings__BaseUrl` | API base URL (for Blazor UI) |

---

## API Endpoints

| Method | Path | Role |
|---|---|---|
| POST | `/api/auth/login` | Public |
| GET | `/api/auth/me` | Any authenticated |
| GET | `/api/sponsorship-types` | Any authenticated |
| GET | `/api/requests` | Requestor / Manager / FinanceAdmin |
| POST | `/api/requests` | Requestor |
| PUT | `/api/requests/{id}` | Requestor |
| POST | `/api/requests/{id}/submit` | Requestor |
| POST | `/api/requests/{id}/cancel` | Requestor |
| POST | `/api/requests/{id}/manager-approve` | Manager |
| POST | `/api/requests/{id}/manager-reject` | Manager |
| POST | `/api/requests/{id}/finance-approve` | FinanceAdmin |
| POST | `/api/requests/{id}/finance-reject` | FinanceAdmin |
| GET | `/api/admin/requests` | SystemAdmin |
| GET | `/api/admin/workflow-history` | SystemAdmin |
| CRUD | `/api/admin/sponsorship-types` | SystemAdmin |

---

## Technical Decisions

| Decision | Rationale |
|---|---|
| **PBKDF2 password hashing** | `BCrypt.Net-Next` was unavailable in the NuGet source mapping; built-in `Rfc2898DeriveBytes` (100K iterations, SHA256) is equally secure |
| **Blazor Server** (not WASM) | Simpler deployment, server-side rendering, no CORS complications between UI and API during development |
| **JWT stored via `ProtectedLocalStorage`** | Secure, encrypted browser storage with per-user keys; avoids exposing raw tokens |
| **Clean Architecture** | Domain → Application → Infrastructure → API; no circular dependencies; easy to swap implementations |
| **EF Core auto-migration** | `MigrateAsync()` on startup ensures the schema is always up to date in deployed environments |

---

## Deployment (Render.com)

### API (Web Service)

- **Build command**: `dotnet publish src/Sponsorship.API -c Release -o publish`
- **Start command**: `dotnet publish/Sponsorship.API.dll`
- **Environment**: set `ConnectionStrings__Default`, `Jwt__Key`, `Jwt__Issuer`, `Jwt__Audience`

### Blazor UI (Web Service)

- **Build command**: `dotnet publish src/Sponsorship.BlazorUI -c Release -o publish`
- **Start command**: `dotnet publish/Sponsorship.BlazorUI.dll`
- **Environment**: set `ApiSettings__BaseUrl` to the deployed API URL
