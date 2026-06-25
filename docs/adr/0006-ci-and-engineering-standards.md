# ADR 0006: CI And Engineering Standards

## Status

Accepted

## Context

The repository needs repeatable quality gates and shared engineering conventions.

## Decision

Use GitHub Actions to run restore, vulnerability scan, build, format verification, EF migration checks, PostgreSQL migration application, and tests.

Use shared repository-level files:

- `.editorconfig` for formatting and naming conventions.
- `.gitattributes` for line endings.
- `Directory.Build.props` for common .NET properties and analyzers.
- `Directory.Packages.props` for central package version management.

Warnings are treated as errors when `CI=true`.

## Consequences

Build and style expectations are consistent locally and in CI. Package versions are managed centrally. Pull requests can be blocked by required status checks in GitHub branch protection.
