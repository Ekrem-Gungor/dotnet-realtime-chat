# Deployment Checklist

Siglora's images and Compose topology are ready for a deployment rehearsal. The homelab rollout is intentionally deferred until the target server is available. This document is a checklist, not evidence of a live deployment.

## 1. Target Design

- Publish one HTTPS hostname through the homelab reverse proxy.
- Route public traffic to the `web` container only.
- Keep API, SQL Server, and Redis on an internal Docker network where practical.
- Let Nginx proxy `/api` and `/chatHub` to the API.
- Persist SQL Server data on storage included in the backup plan.
- Keep Redis private; decide whether message durability is required before rollout.

## 2. Production Configuration

Do not reuse `.env.example`. Create server-side secrets outside Git for:

- SQL Server administrator password
- JWT signing key with sufficient entropy
- Any production demo or operator credential
- Public application origin

Before rollout:

- Set `ASPNETCORE_ENVIRONMENT=Production`.
- Disable `DemoIdentity__Enabled`.
- Disable automatic migrations and run them as an explicit release step.
- Restrict published SQL Server, Redis, and API ports unless operational access requires them.
- Configure the external reverse proxy to preserve WebSocket upgrades for `/chatHub`.
- Configure forwarded headers deliberately for the chosen proxy topology.
- Verify cookie `Secure`, `SameSite`, domain, and path behavior over HTTPS.

The repository Compose file is optimized for a local portfolio demo. Create a server-specific override instead of placing production values in the tracked file.

## 3. Pre-deployment Gate

Run from a clean checkout:

```bash
dotnet restore RealtimeChat.sln
dotnet build RealtimeChat.sln --configuration Release --no-restore
dotnet test RealtimeChat.sln --configuration Release --no-build --no-restore
docker compose --env-file .env config --quiet
docker compose --env-file .env build
```

Confirm that GitHub Actions is green for the release commit.

## 4. Database and Backup

1. Take a verified SQL Server backup before migration.
2. Record the current application image tag and migration ID.
3. Run migrations as a controlled one-off task.
4. Verify the resulting migration ID.
5. Confirm restore steps on a disposable database before exposing the service.

Redis currently stores recent messages and quota state. Treat that data as disposable unless persistence and backup are added explicitly.

## 5. Smoke Test

After startup, verify:

1. Web root and SPA route refresh return successfully.
2. `/health` is healthy through the intended route.
3. Login, session restoration, and logout work over HTTPS.
4. Two independent browser sessions receive one copy of each message.
5. Enter sends; Shift+Enter inserts a line break.
6. Multiple tabs keep the user online until the last tab closes.
7. Restarting the API moves the UI through offline/reconnecting/live without F5.
8. Quota and validation failures render the expected Problem Details messages.
9. No secrets or JWT values appear in browser storage, logs, or repository files.

## 6. Observability Gate

Before calling the service production-ready, add or confirm:

- Centralized structured logs
- Uptime checks for web and API health endpoints
- Alerting for repeated API restarts and database failures
- Disk-capacity monitoring for SQL Server backups and volumes
- A retention policy for logs, backups, and Redis messages

## 7. Rollback

Keep the previous immutable image tag and database backup. If the smoke test fails:

1. Stop accepting public traffic.
2. Roll back the application images.
3. Restore the database only when the migration is not backward-compatible.
4. Repeat the health, authentication, and two-client messaging checks.
5. Record the failure and remediation before retrying.

## Deferred Scale Work

The current presence tracker is in memory and correct for one API instance. Horizontal scaling requires a shared connection/presence store plus a SignalR backplane or managed SignalR service.
