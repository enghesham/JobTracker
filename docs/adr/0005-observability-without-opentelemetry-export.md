# ADR 0005: Observability Without OpenTelemetry Export

## Status

Accepted

## Context

The application needs structured logging, request correlation, handler duration tracking, reminder metrics, and database error metrics. There is not yet a concrete observability backend.

## Decision

Use built-in `ILogger`, logging scopes, correlation ID middleware, MediatR pipeline logging, and `System.Diagnostics.Metrics`.

Do not add OpenTelemetry packages until there is a real exporter/backend requirement.

## Consequences

The application produces structured logs and metrics with low dependency overhead. OpenTelemetry can be added later at the API composition root without changing the core application flow.
