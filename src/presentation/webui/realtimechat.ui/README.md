# Siglora Web Client

React and TypeScript client for the Siglora real-time chat API.

## Features

- HttpOnly cookie authentication and session restoration
- Protected and public-only routes
- Recent message history and message creation
- SignalR delivery with automatic reconnect
- Online-user presence and connection status
- Message de-duplication across HTTP and SignalR
- Enter-to-send and Shift+Enter multiline input
- Problem Details, validation, quota, session, and connection error states
- Responsive chat interface
- Vitest and Testing Library coverage

## Development

Requirements: Node.js 22, npm, and an API at `http://localhost:5258`.

```powershell
Copy-Item .env.example .env.development
npm ci
npm run dev
```

On Bash, use `cp .env.example .env.development`.

The development file contains:

```env
VITE_API_BASE_URL=http://localhost:5258
VITE_SIGNALR_HUB_URL=http://localhost:5258/chatHub
```

Only `VITE_` variables are exposed to browser code. Never place secrets, credentials, signing keys, or connection strings in frontend environment files.

## Quality Checks

```bash
npm ci
npm run lint
npm run build
npm test
```

Use `npm run test:watch` during development.

## Production Container

The multi-stage Dockerfile compiles the client with Node and serves the static output through Nginx:

```bash
docker build -t siglora-web .
docker run --rm -p 5173:8080 siglora-web
```

Production defaults are root-relative:

- `VITE_API_BASE_URL=/`
- `VITE_SIGNALR_HUB_URL=/chatHub`

Nginx proxies `/api` and `/chatHub` to the Compose service named `api`. Use the root `compose.yml` to run the complete stack.

## Routes

| Route | Access | Purpose |
| --- | --- | --- |
| `/login` | Anonymous | Authenticate with the API |
| `/chat` | Authenticated | Message history, real-time delivery, and presence |

## Source Layout

```text
src/
  app/           Routing and route guards
  features/      Authentication and chat features
  shared/        API, configuration, and shared utilities
  styles/        Global styles
  test/          Shared test setup
  main.tsx       Application entry point
```

## Authentication Model

The API writes the JWT to an HttpOnly cookie. Requests and the SignalR connection send credentials automatically; the token is never stored in local storage or session storage. On startup, `/api/Auth/session` distinguishes an anonymous `401` from infrastructure failures.
