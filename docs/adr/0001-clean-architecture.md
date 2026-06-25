# ADR 0001: Clean Architecture Project Boundaries

## Status

Accepted

## Context

The project needs a structure that keeps business rules isolated from API and persistence concerns while still remaining practical for a small product.

## Decision

Use four main runtime projects:

- `JobTracker.Domain` for entities and domain invariants.
- `JobTracker.Application` for use cases, validation, result types, and abstractions.
- `JobTracker.Infrastructure` for EF Core, stores, read services, authentication services, background jobs, and provider-specific integrations.
- `JobTracker.Api` for HTTP controllers, middleware, authentication, rate limiting, and response mapping.

Dependency direction:

- Api -> Application + Infrastructure
- Application -> Domain
- Infrastructure -> Application + Domain
- Domain -> none

## Consequences

Business logic can be tested without ASP.NET Core or EF Core. Infrastructure can change providers with limited impact. The API remains thin and focused on HTTP concerns.
