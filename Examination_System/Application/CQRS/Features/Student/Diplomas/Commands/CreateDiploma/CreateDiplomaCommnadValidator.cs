using FluentValidation;

namespace Application.CQRS.Features.Student.Diplomas.Commands.CreateDiploma
{
    public class CreateDiplomaCommnadValidator : AbstractValidator<CreateDiplomaCommand>
    {
        public CreateDiplomaCommnadValidator()
        {
            RuleFor(x => x.Title)
               .NotEmpty().WithMessage("Title is required.")
              .Length(3, 200).WithMessage("Title cannot be less than 2 characters and not exceed 200 characters.");
            RuleFor(x => x.Description)
               .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

        }
    }
}
