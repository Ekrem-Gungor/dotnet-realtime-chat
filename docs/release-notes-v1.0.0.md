# Siglora v1.0.0

**Release date:** 2026-09-26

**Status:** Portfolio baseline; homelab deployment deferred

Siglora v1.0.0 completes the planned portfolio scope for an authenticated real-time chat application. It combines a layered .NET backend with a tested React client, resilient SignalR delivery, Redis-backed messaging controls, and reproducible container packaging.

## What Is Included

- JWT authentication transported to browsers with an HttpOnly cookie
- Session restoration and protected frontend routes
- Redis-backed recent message history and atomic message quota
- Real-time SignalR message broadcasts with a shared HTTP/SignalR DTO
- Automatic client reconnect after an API interruption
- Connection-aware online presence for multiple tabs
- Client-side message de-duplication by stable message ID
- Standard validation and Problem Details responses
- Multi-stage frontend image served by Nginx
- Full local Compose topology for web, API, SQL Server, and Redis
- Backend and frontend CI quality gates, including container builds

## Verification Summary

The frontend release candidate passed:

- ESLint
- TypeScript and Vite production build
- 18 Vitest tests across 4 test files

The repository CI definition also runs .NET Release build/tests with Redis, validates Compose, and builds both API and web images. Those .NET and Docker commands must be rerun in GitHub Actions or on a workstation with the required runtimes before tagging.

## Run Locally

```bash
cp .env.example .env
# Replace example passwords and JWT key in .env.
docker compose up --build -d
```

Open `http://localhost:5173` and authenticate with the configured demo account.

## Tagging Gate

Create the tag only after CI is green and the container smoke test succeeds:

```bash
git tag -a v1.0.0 -m "Siglora v1.0.0"
git push origin v1.0.0
```

Suggested GitHub release title: `Siglora v1.0.0 — portfolio baseline`

## Deployment Note

No public or homelab deployment is claimed for this release. Follow [the deployment checklist](deployment.md) when the server is available, especially the production environment, secret-management, TLS, migration, backup, and rollback gates.

## Known Boundaries

- One API replica only for correct presence behavior
- No public registration or password recovery flow
- Redis message retention trimming is not implemented
- Production observability and load testing are deferred to deployment
