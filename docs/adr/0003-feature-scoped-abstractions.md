# ADR 0003: Feature-Scoped Abstractions

## Status

Accepted

## Context

A large `Application/Common/Interfaces` folder can become a dumping ground as the product grows.

## Decision

Keep only truly shared abstractions in `Application/Common/Interfaces`, such as current user and unit of work. Move feature-specific stores and read service interfaces into their owning feature modules.

Examples:

- Companies own `ICompanyStore` and `ICompanyReadService`.
- Job applications own `IJobApplicationStore` and `IJobApplicationReadService`.
- Auth owns user store, password hashing, and JWT abstractions.

## Consequences

Feature code is easier to discover. Common abstractions remain small. The architecture avoids generic repositories and avoids exposing `IQueryable` outside Infrastructure.
