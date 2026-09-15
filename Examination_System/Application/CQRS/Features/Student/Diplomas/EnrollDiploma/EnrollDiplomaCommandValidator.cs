using FluentValidation;

namespace Application.CQRS.Features.Student.Diplomas.EnrollDiploma
{
    public class EnrollDiplomaCommandValidator : AbstractValidator<EnrollDiplomaCommand>
    {
        public EnrollDiplomaCommandValidator()
        {
            RuleFor(x => x.DiplomaId)
                .NotEqual(Guid.Empty)
                .WithMessage("Invalid diploma ID.");
        }
    }
}