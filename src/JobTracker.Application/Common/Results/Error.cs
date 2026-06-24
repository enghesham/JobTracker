namespace JobTracker.Application.Common.Results;

public sealed record Error(string Code, string Title, string Detail, ErrorType Type)
{
    public static Error Validation(string code, string title, string detail) => new(code, title, detail, ErrorType.Validation);
    public static Error NotFound(string code, string title, string detail) => new(code, title, detail, ErrorType.NotFound);
    public static Error Conflict(string code, string title, string detail) => new(code, title, detail, ErrorType.Conflict);
    public static Error Unauthorized(string code, string title, string detail) => new(code, title, detail, ErrorType.Unauthorized);
    public static Error Forbidden(string code, string title, string detail) => new(code, title, detail, ErrorType.Forbidden);
    public static Error Failure(string code, string title, string detail) => new(code, title, detail, ErrorType.Failure);
}
