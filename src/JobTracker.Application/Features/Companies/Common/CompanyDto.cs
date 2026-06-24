namespace JobTracker.Application.Features.Companies.Common;

public sealed record CompanyDto(Guid Id, string Name, string? Website, string? Location);
