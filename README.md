# Checklist Desktop Todo App

Base foundation project for a local desktop todo/checklist app using:

- Backend: .NET 8 Web API + EF Core + SQLite
- Frontend: React + TypeScript + Vite
- Desktop shell: Electron
- Integration: Google Calendar reminder link generation

This version focuses on clean structure, clear comments, and easy extensibility.

## Folder Structure

```text
.
├── backend
│   ├── Controllers
│   ├── Data
│   ├── DTOs
│   ├── Enums
│   ├── Models
│   ├── Repositories
│   ├── Services
│   ├── Checklist.Api.csproj
│   └── Program.cs
├── frontend
│   ├── src
│   │   ├── components
│   │   ├── hooks
│   │   ├── pages
│   │   ├── services
│   │   └── types
│   └── package.json
├── electron
│   ├── main.js
│   ├── preload.js
│   └── package.json
├── .gitignore
└── README.md
```

## Prerequisites

Install these tools first:

- .NET SDK 8.0+
- Node.js 20+ and npm

## Backend Setup (.NET 8)

1. Go to backend folder:

```bash
cd backend
```

2. Restore packages:

```bash
dotnet restore
```

3. Create/update database:

```bash
dotnet ef database update
```

Notes:

- If you do not have EF CLI installed yet: `dotnet tool install --global dotnet-ef`
- The app also calls `EnsureCreated` on startup for local convenience.
- SQLite database is stored in the local user app data folder (`ChecklistDesktop/checklist.db`).

4. Run the backend API:

```bash
dotnet run --urls http://localhost:5050
```

Swagger UI is available in development mode.

## Frontend Setup (React + TypeScript)

1. Go to frontend folder:

```bash
cd frontend
```

2. Install dependencies:

```bash
npm install
```

3. Run dev server:

```bash
npm run dev
```

4. Optional API URL override:

- Set `VITE_API_BASE_URL` in a frontend `.env` file.
- Default is `http://localhost:5050`.

## Electron Setup

1. Go to electron folder:

```bash
cd electron
```

2. Install dependencies:

```bash
npm install
```

3. Run Electron in development:

```bash
npm run dev
```

What happens in dev:

- Electron starts and spawns the backend with `dotnet run`
- Electron loads the frontend dev server URL (`http://localhost:5173`)

## Run Everything From Repo Root

Install root tooling:

```bash
npm install
```

Run frontend + electron together:

```bash
npm run dev
```

Note: backend process is launched by Electron main process.

## Google Calendar Reminder Behavior

- The app does not manage Google sign-in.
- Clicking Add Google Calendar Reminder builds a prefilled Google Calendar event URL and opens it in your default browser.
- If the user is already signed in to Google in that browser, they can save the reminder immediately.
- If the user is not signed in, Google will prompt for login in the browser.

## Build Windows Installer

1. Build frontend assets:

```bash
cd frontend
npm run build
```

2. Build Electron package:

```bash
cd ../electron
npm run build
```

The packaged output is written to `electron/dist`.

## CI/CD (GitHub Actions)

This repository includes a Windows release pipeline at:

- `.github/workflows/release-windows.yml`

What it does:

1. Builds frontend assets (`frontend/dist`)
2. Publishes backend as a self-contained Windows executable (`backend/publish-win`)
3. Packages Electron with `electron-builder`
4. Uploads `.exe` installer artifacts
5. Automatically attaches installer files to GitHub Releases

How to use it:

1. Push your changes to GitHub
2. Create and push a version tag (for example `v0.1.0`)

```bash
git tag -a v0.1.0 -m "Release v0.1.0"
git push origin v0.1.0
```

3. Wait for the `Build Windows Release` workflow to finish
4. Download the generated installer from the Release assets

You can also run it manually from the Actions tab via `workflow_dispatch`.

## API Endpoints (Summary)

- `GET /api/todos`
- `GET /api/todos/{id}`
- `POST /api/todos`
- `PUT /api/todos/{id}`
- `PATCH /api/todos/{id}/toggle`
- `DELETE /api/todos/{id}`
- `POST /api/google-calendar/todos/{todoId}/reminder`
