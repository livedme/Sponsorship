# Sponsorship Request Approval Workflow — Execution Plan

**Assessment Deadline:** 4 Days | **Estimated Effort:** ~6 hours/day  
**Stack:** .NET 10 · Blazor Server · PostgreSQL · JWT · MudBlazor  
**Deliverables:** Live URL · Swagger URL · Git Repo · README · Seed Accounts

---

## Tech Stack Decision

| Layer          | Choice                 | Reason                                              |
|----------------|------------------------|-----------------------------------------------------|
| Architecture   | Clean Architecture     | Use Clean Architecture                              |
| Frontend       | Blazor Server          | Fastest for .NET developer; shared models with API  |
| Backend API    | ASP.NET Core Web API   | Industry standard; clean REST; Swagger built-in     |
| ORM            | Entity Framework Core  | Rapid schema migration; LINQ-friendly               |
| Database       | PostgreSQL             | Free tier on Render; robust for workflow data       |
| Auth           | JWT + Role Claims      | Stateless; easy role-based authorization            |
| UI Library     | MudBlazor              | Professional enterprise look out of the box         |
| Hosting        | Render.com             | Free tier; PostgreSQL + Web Service in one place    |
| API Docs       | Swagger / OpenAPI      | Required deliverable                                |

---

## Solution Structure

```
Sponsorship.sln
├── src/
│   ├── Sponsorship.API              ← ASP.NET Core Web API
│   ├── Sponsorship.Domain           ← Entities, Enums, Interfaces
│   ├── Sponsorship.Application      ← Services, DTOs, Workflow logic
│   ├── Sponsorship.Infrastructure   ← EF Core, Repositories, Migrations
│   └── Sponsorship.BlazorUI         ← Blazor Server frontend
└── README.md
```

---

## Database Schema

### Users
| Column       | Type     |
|--------------|----------|
| Id           | uuid     |
| Email        | string   |
| PasswordHash | string   |
| FullName     | string   |
| Department   | string   |
| Role         | enum     |
| CreatedAt    | datetime |

### SponsorshipTypes
| Column | Type   |
|--------|--------|
| Id     | int    |
| Name   | string |

### SponsorshipRequests
| Column                  | Type     |
|-------------------------|----------|
| Id                      | uuid     |
| RequestTitle            | string   |
| RequestorId             | uuid FK  |
| RequestorName           | string   |
| Department              | string   |
| SponsorshipTypeId       | int FK   |
| EventName               | string   |
| EventDate               | date     |
| RequestedAmount         | decimal  |
| Purpose                 | text     |
| ExpectedBusinessBenefit | text     |
| Remarks                 | text     |
| SupportingDocumentUrl   | string?  |
| Status                  | enum     |
| CreatedAt               | datetime |
| UpdatedAt               | datetime |

### WorkflowHistory
| Column         | Type     |
|----------------|----------|
| Id             | uuid     |
| RequestId      | uuid FK  |
| ActionById     | uuid FK  |
| ActionByName   | string   |
| PreviousStatus | enum     |
| NewStatus      | enum     |
| Remarks        | text     |
| Timestamp      | datetime |

---

## Status Enum & Workflow Transitions

```
Draft
  └─[Submit]──────────────► PendingManagerApproval
                                  ├─[Manager Approve]──► PendingFinanceReview
                                  │                           ├─[Finance Approve]──► Approved
                                  │                           └─[Finance Reject]───► Rejected
                                  └─[Manager Reject]───► Rejected
Draft / PendingManagerApproval
  └─[Requestor Cancel]─────► Cancelled
```

---

## Seed Test Accounts

| Email               | Password  | Role         |
|---------------------|-----------|--------------|
| requestor@test.com  | Test@1234 | Requestor    |
| manager@test.com    | Test@1234 | Manager      |
| finance@test.com    | Test@1234 | FinanceAdmin |
| admin@test.com      | Test@1234 | SystemAdmin  |

---

## API Endpoints

### Auth
```
POST   /api/auth/login
GET    /api/auth/me
```

### Sponsorship Requests
```
GET    /api/requests               (role-filtered: own / pending / all)
GET    /api/requests/{id}
POST   /api/requests               (Requestor — create draft)
PUT    /api/requests/{id}          (Requestor — edit draft)
POST   /api/requests/{id}/submit   (Draft → PendingManagerApproval)
POST   /api/requests/{id}/cancel   (Requestor — cancel before approval)
```

### Approval Actions
```
POST   /api/requests/{id}/manager-approve    [Manager]
POST   /api/requests/{id}/manager-reject     [Manager]
POST   /api/requests/{id}/finance-approve    [FinanceAdmin]
POST   /api/requests/{id}/finance-reject     [FinanceAdmin]
```

### Admin
```
GET    /api/requests/all                     [SystemAdmin]
GET    /api/workflow-history                 [SystemAdmin]
GET    /api/sponsorship-types
POST   /api/sponsorship-types               [SystemAdmin]
PUT    /api/sponsorship-types/{id}          [SystemAdmin]
DELETE /api/sponsorship-types/{id}          [SystemAdmin]
```

---

## Blazor UI Pages

| Route                       | Page                             | Roles             |
|-----------------------------|----------------------------------|-------------------|
| `/login`                    | Login                            | Public            |
| `/requests`                 | My Requests list                 | Requestor         |
| `/requests/new`             | Create / Edit Request form       | Requestor         |
| `/requests/{id}`            | Request Detail + Workflow Timeline | All authenticated |
| `/manager/pending`          | Manager Approval queue           | Manager           |
| `/finance/pending`          | Finance Review queue             | FinanceAdmin      |
| `/admin/requests`           | All Requests                     | SystemAdmin       |
| `/admin/workflow-history`   | Full Audit Log                   | SystemAdmin       |
| `/admin/sponsorship-types`  | Manage Sponsorship Types         | SystemAdmin       |

---

## Status Color Coding

| Status                   | Badge Color |
|--------------------------|-------------|
| Draft                    | Gray        |
| Pending Manager Approval | Orange      |
| Pending Finance Review   | Blue        |
| Approved                 | Green       |
| Rejected                 | Red         |
| Cancelled                | Dark Gray   |

---

## 4-Day Execution Timeline

---

### Day 1 — Foundation (Backend + Auth + Database)

**Goal:** Running API with JWT auth, seeded database, all entities migrated.

#### Morning (3 h)
- [ ] `dotnet new sln -n Sponsorship`
- [ ] Create projects: `Domain`, `Application`, `Infrastructure`, `API`
- [ ] Wire project references: `API → Application → Domain`; `Infrastructure → Application`
- [ ] Install NuGet packages:
  - `Npgsql.EntityFrameworkCore.PostgreSQL`
  - `Microsoft.EntityFrameworkCore.Design`
  - `Microsoft.AspNetCore.Authentication.JwtBearer`
  - `BCrypt.Net-Next`
  - `Swashbuckle.AspNetCore`

#### Afternoon (3 h)
- [ ] Define Domain entities: `User`, `SponsorshipRequest`, `WorkflowHistory`, `SponsorshipType`
- [ ] Define `RequestStatus` enum (Draft, PendingManagerApproval, PendingFinanceReview, Approved, Rejected, Cancelled)
- [ ] Create `AppDbContext` and register all DbSets
- [ ] `dotnet ef migrations add InitialCreate` → `dotnet ef database update`
- [ ] Seed 4 test users with BCrypt-hashed passwords and role assignments
- [ ] Implement `POST /api/auth/login` → returns JWT with email + role claims
- [ ] Verify login and token in Swagger

**End-of-Day Check:** Login works, JWT returned, Swagger accessible at `/swagger`.

---

### Day 2 — Core Business Logic (Workflow + APIs)

**Goal:** All REST endpoints working, workflow state machine enforced, audit trail written on every transition.

#### Morning (3 h)
- [ ] Implement `SponsorshipRequestService`:
  - `CreateDraft()` — status = Draft, owned by requestor
  - `Submit(id)` — guard: caller is owner + status is Draft
  - `Cancel(id)` — guard: caller is owner + status is Draft or PendingManagerApproval
  - `ManagerApprove(id, remarks)` — guard: Manager role + status is PendingManagerApproval
  - `ManagerReject(id, remarks)` — same guard
  - `FinanceApprove(id, remarks)` — guard: FinanceAdmin role + status is PendingFinanceReview
  - `FinanceReject(id, remarks)` — same guard
- [ ] Each state change appends a `WorkflowHistory` record (previousStatus, newStatus, actor, remarks, timestamp)
- [ ] Invalid transitions return `400 Bad Request`; wrong role returns `403 Forbidden`

#### Afternoon (3 h)
- [ ] Implement all Request and Approval controllers with `[Authorize(Roles = "...")]`
- [ ] Filter `GET /api/requests` by caller role (own / pending / all)
- [ ] Implement Admin endpoints: all requests, audit log, sponsorship-type CRUD
- [ ] Test every endpoint in Swagger under each role's JWT

**End-of-Day Check:** Full Draft → Approved flow works. Rejection path works. WorkflowHistory populated.

---

### Day 3 — Blazor Frontend

**Goal:** All role dashboards functional, forms validated, status colors applied, audit timeline visible.

#### Setup (30 min)
- [ ] `dotnet new blazorserver -n Sponsorship.BlazorUI`
- [ ] Install and configure `MudBlazor`
- [ ] Register `HttpClient` with API base URL
- [ ] Implement `CustomAuthStateProvider` — reads JWT from `localStorage`, exposes `ClaimsPrincipal`

#### Pages (5 h)
- [ ] **Login page** — call `/api/auth/login`, store token, redirect to role-specific dashboard
- [ ] **Requestor dashboard** — table of own requests with status badges; New / Edit / Submit / Cancel buttons
- [ ] **Request form** — all required fields (title, dept, sponsor type, event name, event date, amount, purpose), optional fields (benefit, remarks, document URL), client-side validation
- [ ] **Request detail page** — field summary + chronological Workflow Timeline (actor · action · timestamp · remarks)
- [ ] **Manager dashboard** — pending approval queue; Approve / Reject modal with remarks field
- [ ] **Finance dashboard** — pending finance review queue; Approve / Reject modal
- [ ] **Admin — all requests** — full table with status filter
- [ ] **Admin — audit log** — all WorkflowHistory records
- [ ] **Admin — sponsorship types** — inline add / edit / delete

**End-of-Day Check:** All 4 roles can log in, perform their actions, and see correct filtered data.

---

### Day 4 — Deployment, Documentation & Polish

**Goal:** Live URL, Swagger URL, clean README, all deliverables confirmed.

#### Morning (3 h) — Deployment
- [ ] Push full codebase to GitHub with clean commit history
- [ ] Provision Render PostgreSQL instance; copy internal connection string
- [ ] Create Render Web Service for API:
  - Build: `dotnet publish -c Release -o out`
  - Start: `dotnet out/Sponsorship.API.dll`
  - Env vars: `ConnectionStrings__Default`, `Jwt__Key`, `Jwt__Issuer`, `ASPNETCORE_ENVIRONMENT=Production`
- [ ] Apply migrations on startup (`app.ApplyMigrations()` in `Program.cs`)
- [ ] Deploy Blazor UI (same or separate Render service)
- [ ] Confirm `/swagger` is publicly accessible
- [ ] Smoke-test all 4 seed accounts on live URL

#### Afternoon (3 h) — README + Polish
- [ ] Write `README.md` with:
  - Architecture overview (text diagram)
  - Local setup steps (backend + frontend + DB)
  - Environment variable reference
  - Live URLs (frontend, API, Swagger)
  - Test account table with passwords
  - Workflow state diagram
  - RBAC explanation
  - Assumptions & tradeoffs
- [ ] UI polish: loading spinners, empty-state messages, consistent spacing, form error display
- [ ] Final end-to-end smoke test: full approval path + rejection path under all 4 roles

**End-of-Day Check:** Live app accessible, README complete, repo link ready to submit.

---

## Assumptions & Tradeoffs (README Template)

```
- File upload omitted to stay within assessment time; field exists in schema as nullable URL.
- Single manager approval — no multi-manager chain needed for this scope.
- No email/push notifications — status tracking via audit log is sufficient.
- No multi-tenancy or department hierarchy.
- Blazor Server chosen over React/Angular for faster .NET developer delivery.
- Password reset flow not implemented.
```

---

## Minimum Viable Success Criteria

| Requirement                                       | Done |
|---------------------------------------------------|------|
| All 4 roles can log in                            | ☐    |
| Workflow transitions enforced and correct         | ☐    |
| Audit / workflow history recorded per action      | ☐    |
| Requestor can create, edit, submit, cancel        | ☐    |
| Manager can approve / reject with remarks         | ☐    |
| Finance can final approve / reject with remarks   | ☐    |
| SystemAdmin sees all requests and full audit log  | ☐    |
| Live frontend URL accessible                      | ☐    |
| Live API + Swagger URL accessible                 | ☐    |
| README covers architecture, setup, accounts       | ☐    |
| Git repo with meaningful commit history           | ☐    |

---

## Optional Enhancements (Only If Time Permits)

- `FluentValidation` — server-side request validation
- `MediatR` — CQRS-style command/query handlers
- `Serilog` — structured logging with request tracing
- `Docker` + `docker-compose` — local dev environment
- GitHub Actions CI — build + test on push

> Do **not** add these if they risk missing the deadline. Clean, complete delivery beats feature creep.

---

## Evaluation Priority (Reviewer Perspective)

| Area                       | Weight    |
|----------------------------|-----------|
| Workflow logic correctness | Very High |
| Code structure / clean arch| Very High |
| Audit history              | High      |
| RBAC implementation        | High      |
| API design & naming        | High      |
| UI usability               | Medium    |
| Advanced patterns          | Low       |
