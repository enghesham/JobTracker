# ADR 0004: TimeProvider And Auditing

## Status

Accepted

## Context

Direct use of `DateTime.UtcNow` makes code harder to test and can leak infrastructure concerns into domain logic. Audit timestamps should not be set manually by application handlers.

## Decision

Use .NET `TimeProvider` for reading current time in Application and Infrastructure. Domain methods receive time values as parameters where business rules require time comparisons.

Use `DateTimeOffset` for API-facing and persisted UTC timestamps.

Use an EF Core `SaveChangesInterceptor` to set `CreatedAtUtc` and `UpdatedAtUtc`. The timestamp mutation methods on `BaseEntity` are internal and visible only to Infrastructure.

## Consequences

Time-dependent behavior is testable with fake time providers. Audit fields are controlled centrally by Infrastructure. Domain code does not read the system clock.
