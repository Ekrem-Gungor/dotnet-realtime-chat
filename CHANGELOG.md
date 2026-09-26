# Changelog

All notable changes to Siglora are documented here.

## [1.0.0] - 2026-09-26

### Added

- React and TypeScript web client with protected routing and cookie-based sessions
- HTTP message history and creation flows
- SignalR message delivery, reconnect behavior, connection status, and online presence
- Connection-aware multi-tab presence tracking
- Redis-backed recent messages and atomic per-user quota
- Standard Problem Details responses and validation pipeline
- Production multi-stage frontend image with Nginx reverse proxy
- Full local Compose stack for web, API, SQL Server, and Redis
- Frontend image validation in CI
- Deployment checklist and release documentation

### Changed

- Standardized HTTP and SignalR message payloads on the complete message DTO
- Added client-side message de-duplication by message ID
- Allowed frontend configuration to use absolute development URLs or root-relative production paths
- Updated the portfolio documentation to match the implemented system

### Fixed

- Initial SignalR connection failures now retry after the API returns
- Closing one of multiple tabs no longer marks the user offline prematurely
- HTTP and SignalR delivery no longer render the sender's message twice
