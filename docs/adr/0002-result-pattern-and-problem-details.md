# ADR 0002: Result Pattern And ProblemDetails

## Status

Accepted

## Context

Expected application outcomes such as not found, conflict, unauthorized, and validation failures should not depend on exceptions for normal control flow. API errors also need a consistent response shape.

## Decision

Application handlers return `Result<T>` with an `Error` containing code, title, detail, and error type. API extensions map these results to ASP.NET Core `ProblemDetails`.

Unexpected exceptions are still handled by centralized middleware.

## Consequences

Controllers stay consistent and simple. Expected failures are explicit in handler signatures. API responses include stable error codes and trace identifiers.
