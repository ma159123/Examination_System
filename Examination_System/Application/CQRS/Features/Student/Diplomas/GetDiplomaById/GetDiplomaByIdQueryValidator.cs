using FluentValidation;

namespace Application.CQRS.Features.Student.Diplomas.GetDiplomaById
{
    public class GetDiplomaByIdQueryValidator : AbstractValidator<GetDiplomaByIdQuery>
    {
        public GetDiplomaByIdQueryValidator()
        {
            RuleFor(x => x.DiplomaId)
                .NotEqual(Guid.Empty)
                .WithMessage("Diploma ID is required.");
        }
    }
}
