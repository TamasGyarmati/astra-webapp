# Venus

A university subject and teacher management system. The point: create, edit and delete subjects and teachers, and connect the two. Angular frontend, ASP.NET Core backend, MSSQL database, JWT authentication.

## Contents

- [Technologies](#technologies)
- [Project structure](#project-structure)
- [Data model](#data-model)
- [API endpoints](#api-endpoints)
- [Features](#features)
- [How to use](#how-to-use)

## Technologies

| Layer | Technology | Version |
| --- | --- | --- |
| Frontend | Angular (standalone, signals) | 21.2 |
| Frontend | TypeScript | ~5.9 |
| Frontend | Angular Material + CDK | 21.2 |
| Frontend | Bootstrap, GSAP | 5.3 / 3.15 |
| Frontend | Vitest, jsdom | 4.0 / 28 |
| Backend | ASP.NET Core | net10.0 |
| Backend | Entity Framework Core (SqlServer) | 10.0 |
| Backend | JWT Bearer auth | 10.0 |
| Backend | Swashbuckle (Swagger UI) | 10.2 |
| Database | Microsoft SQL Server | 2022 (Docker) |
| Tooling | npm / .NET CLI | 11.6 / 10.0 |

## Project structure

```
Venus/
├── Backend/
│   └── Students/
│       ├── Students.sln
│       └── Students.App/
│           ├── Program.cs              # DI, JWT, CORS, DbContext, Swagger
│           ├── Data/AppDbContext.cs
│           ├── Models/                 # User, Student, Subject, Teacher, School
│           ├── Controllers/            # Auth, School, Students, Subjects, Teacher
│           └── Migrations/             # EF Core migrations
└── Frontend/
    └── teacher-subject-manager/
        └── src/app/
            ├── app.routes.ts          # routes + canActivate guards
            ├── api.service.ts         # login state signals, JWT check
            ├── _env/env.ts            # API URLs + localStorage keys
            ├── _models/               # TypeScript interfaces
            ├── _shared/               # Material imports, shared SCSS
            ├── home/                  # landing, cards
            ├── login/ register/ logout/
            ├── list-teachers/ list-subjects/
            ├── create-teacher/ create-subject/
            ├── update-teachers/ update-subjects/
            └── connect-teacher-to-subject/
```

## Data model

- **User** – email, password (PBKDF2 + salt), roles
- **Subject** – name, Neptun code, credit, is exam, image, registered student count
- **Teacher** – name, Neptun code, birth year, image
- **Student** – name, is active, credits, active semester count
- **Teacher ↔ Subject** – many-to-many via navigation properties

## API endpoints

| Method | Path | Description |
| --- | --- | --- |
| POST | `/api/auth/register` | registration, 409 if email is taken |
| POST | `/api/auth/login` | login, returns JWT + expiration |
| GET | `/api/auth` | list users |
| GET/POST/PUT/DELETE | `/api/teacher`, `/api/teacher/{id}` | teacher CRUD |
| GET/POST/PUT/DELETE | `/api/subjects`, `/api/subjects/{id}` | subject CRUD |
| GET/POST/PUT/DELETE | `/api/students`, `/api/students/{id}` | student CRUD |
| POST | `/api/school` | link teacher to subject |
| DELETE | `/api/school` | unlink teacher from subject |

Swagger UI: `http://localhost:5500/swagger`

## Features

**Authentication**
- Register and login, 8 hour JWT lifetime
- Passwords stored with PBKDF2-SHA256, 100 000 iterations, salt, fixed-time comparison
- Token and expiration go into `localStorage`, the client checks the session against the expiration date
- Route guards: every page is protected with `canActivate`, except `home`, `login`, `register`

**Subjects and teachers**
- Full CRUD on both entities, listing, editing, deleting
- When editing a subject, teachers can be assigned to it
- When editing a teacher, the subject list is editable too

**Connecting**
- `POST /api/school` can link multiple subjects in one call
- The response separates the outcome: `added`, `alreadyLinked`, `notFound`
- The DELETE side breaks the links the same way, with `removed` and `notLinked` lists

**Home page**
Eight Material cards describing the project: Features, Technologies, UI & Styling, Data Management, Architecture, Developer Experience, Security, API Integration. Each one uses chips to surface the key technologies, so Authentication, Create, Update, Delete, Angular, ASP.NET Core, REST API, JWT and the rest are labelled there.

## How to use

### Prerequisites

| Case | Needed |
| --- | --- |
| Frontend only | Node.js 20+ or 22+ (24 recommended), npm 11+ |
| Backend only | .NET SDK 10.0 |
| Full stack | both of the above + Docker Desktop |

The backend targets `net10.0`, so the .NET 9 SDK will not build it.

### 1. Clone

```bash
git clone https://github.com/TamasGyarmati/venus-webapp.git
cd venus-webapp
```

### 2. Database

The backend connects to SQL Server. The easiest option is to run it in Docker:

```bash
docker run -d --name sqlserver \
  -e "MSSQL_SA_PASSWORD=[your_password_goes_here]" \
  -e "ACCEPT_EULA=Y" \
  -p 1433:1433 \
  mcr.microsoft.com/mssql/server:2022-latest
```

This matches the connection string in `appsettings.json`:

```json
"DefaultConnection": "Server=localhost,1433;Database=[your_db_goes_here];User Id=sa;Password=[your_password_goes_here];TrustServerCertificate=True;"
```

If you use a different password or port, edit `Backend/Students/Students.App/appsettings.json`. If the container already exists, this is enough:

```bash
docker start sqlserver
```

### 3. Run the backend

```bash
cd Backend/Students
dotnet restore
dotnet ef database update --project Students.App   # needed on first run
dotnet run --project Students.App
```

If the EF Core CLI is not installed:

```bash
dotnet tool install --global dotnet-ef
```

It starts on `http://localhost:5500` (HTTP) or `https://localhost:7021` (HTTPS), with Swagger UI at `/swagger`. The first run on the HTTPS profile may need the dev certificate trusted in the browser.

### 4. Run the frontend

```bash
cd Frontend/teacher-subject-manager
npm install
npm start
```

Starts on `http://localhost:4200`

### 5. Sign in

1. Open `http://localhost:4200/register` and create an account
2. Log in at `http://localhost:4200/login`
3. The `Manager` menu in the navigation bar holds the teacher and subject lists

### Ports

| Service | Port |
| --- | --- |
| Frontend (Angular dev server) | 4200 |
| Backend (HTTP) | 5500 |
| Backend (HTTPS) | 7021 |
| SQL Server | 1433 |

### Troubleshooting

**`Login failed for user 'sa'`** – SQL Server is not fully started yet, or the password differs. Check the `docker logs sqlserver` output and `appsettings.json`.

**The frontend cannot reach the API** – the URLs in `src/app/_env/env.ts` point at `localhost:5500`. If the backend runs on another port, that file has to change too.

**CORS error** – `Program.cs` only allows port 4200 (4200, 127.0.0.1 and the HTTPS variant). If you run the frontend on a different port, extend that list.

**`dotnet` does not recognise the `net10.0` target** – the installed SDK is too old. Check with:

```bash
dotnet --list-sdks
```

### Commands

```bash
# Frontend
npm start                          # dev server
npm run build                      # production build into dist/
npm test                           # Vitest

# Backend
dotnet build                       # build
dotnet run --project Students.App  # run
```
