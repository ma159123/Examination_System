using Application.CQRS.Features.Student.Diplomas.Queries.GetDiplomaById;
using FluentValidation;

namespace Application.CQRS.Features.Student.QuizFeature.Queries.GetDiplomaQuizes
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
