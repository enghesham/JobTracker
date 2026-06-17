namespace JobTracker.Application.Companies;

public sealed record CompanyDto(Guid Id, string Name, string? Website, string? Location);
