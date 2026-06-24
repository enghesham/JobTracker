using FluentValidation;

namespace JobTracker.Application.Features.Companies.Create;

public sealed class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(command => command.Website)
            .MaximumLength(500);

        RuleFor(command => command.Location)
            .MaximumLength(200);
    }
}
