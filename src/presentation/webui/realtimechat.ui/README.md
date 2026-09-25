# Siglora Web Client

Siglora is the React and TypeScript web client for the RealtimeChat API.

The client currently provides cookie-based authentication, protected routing, session restoration, standardized API error handling, and the initial authenticated application shell.

## Technology Stack

- React
- TypeScript
- Vite
- React Router
- Vitest
- Testing Library
- ESLint

## Current Features

- Username and password login
- HTTP-only authentication cookie support
- Session restoration after page refresh
- Protected and public-only routes
- Logout flow
- RFC Problem Details error handling
- Responsive authenticated application shell
- Route and login-flow tests
- Recent message-history loading
- HTTP-based message creation
- Loading, empty, validation, quota, and connection error states
- Enter-to-send and Shift+Enter multiline input

HTTP messaging is available. Real-time SignalR delivery and online-user presence will be introduced in the next implementation stage.

## Prerequisites

- Node.js 22
- npm
- RealtimeChat API running at `http://localhost:5258`

## Configuration

Copy the example environment file:

### PowerShell

```powershell
Copy-Item .env.example .env.development
```

### Bash

```bash
cp .env.example .env.development
```

The local configuration should contain:

```env
VITE_API_BASE_URL=http://localhost:5258
```

Only variables prefixed with `VITE_` are exposed to browser code. Secrets, connection strings, JWT signing keys, and database credentials must never be placed in frontend environment files.

## Development

Install dependencies:

```bash
npm ci
```

Start the development server:

```bash
npm run dev
```

The client is available at `http://localhost:5173`.

The API must allow this origin and accept credentialed requests.

## Quality Checks

Run linting:

```bash
npm run lint
```

Create a production build:

```bash
npm run build
```

Run the automated tests once:

```bash
npm test
```

Run tests in watch mode:

```bash
npm run test:watch
```

## Project Structure

```text
src/
├── app/           Application routing and route guards
├── features/      Feature-oriented modules such as authentication
├── shared/        Shared API, configuration, and utility code
├── styles/        Global application styles
├── test/          Shared test setup
└── main.tsx       Application entry point
```

## Authentication Model

The API writes the JWT to an HTTP-only cookie after a successful login. The client sends requests with credentials enabled and does not store access tokens in local storage or session storage.

On application startup, the client requests the current authenticated session:

- A successful response restores the user.
- `401 Unauthorized` establishes an anonymous session.
- Infrastructure and unexpected failures remain distinguishable from an unauthenticated state.

## Routes

| Route    | Access              | Purpose                            |
| -------- | ------------------- | ---------------------------------- |
| `/login` | Anonymous users     | Authenticate with the API          |
| `/chat`  | Authenticated users | Authenticated HTTP chat experience |

## CI

GitHub Actions runs the following frontend checks for pushes and pull requests targeting `master`:

```bash
npm ci
npm run lint
npm run build
npm test
```
