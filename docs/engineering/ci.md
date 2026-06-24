# CI Pipeline

The repository uses GitHub Actions workflow `.github/workflows/ci.yml` for pull requests and pushes to `main` or `master`.

The pipeline runs:

- `dotnet restore`
- dependency vulnerability scan with `dotnet list package --vulnerable --include-transitive`
- `dotnet build --no-restore --configuration Release`
- `dotnet format --verify-no-changes`
- EF Core pending migration check
- EF Core migration application against a PostgreSQL service container
- `dotnet test --no-build --configuration Release` with PostgreSQL configuration available

The workflow fails on build errors, test failures, formatting changes, pending model changes not captured by migrations, migration application failures, or vulnerable NuGet packages.

To prevent merges when CI fails, configure GitHub branch protection for `main` and require the `Build, Test, Format, Security, Migrations` status check before merging.

OpenTelemetry is intentionally not required by CI. Metrics and structured logs are implemented in the application; exporting them can be added later when there is an observability backend to receive them.
