using FluentValidation;

namespace Application.CQRS.Features.Student.Diplomas.Commands.DeleteDiploma
{
    public class DeleteDiplomaCommandValidator :
        AbstractValidator<DeleteDiplomaCommand>
    {
        public DeleteDiplomaCommandValidator()
        {
            RuleFor(x => x.DiplomaId)
                .NotEmpty().WithMessage("DiplomaId is required.");
        }
    }
}
