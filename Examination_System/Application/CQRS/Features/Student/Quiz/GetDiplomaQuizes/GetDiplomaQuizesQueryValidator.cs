using Application.CQRS.Features.Student.Diplomas.GetDiplomaById;
using FluentValidation;

namespace Application.CQRS.Features.Student.Quiz.GetDiplomaQuizes
{
    public class GetDiplomaQuizesQueryValidator : AbstractValidator<GetDiplomaByIdQuery>
    {
        public GetDiplomaQuizesQueryValidator()
        {
            RuleFor(x => x.DiplomaId)
                .NotEqual(Guid.Empty)
                .WithMessage("Diploma ID is required.");
        }
    }
}
