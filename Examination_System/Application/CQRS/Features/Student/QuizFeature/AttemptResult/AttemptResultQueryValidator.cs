using FluentValidation;

namespace Application.CQRS.Features.Student.QuizFeature.AttemptResult
{
    public class AttemptResultQueryValidator : AbstractValidator<AttemptResultQuery>
    {
        public AttemptResultQueryValidator()
        {
            RuleFor(x => x.AttemptId)
                .NotEqual(Guid.Empty)
                .WithMessage("Attempt ID is required.");
        }
    }
}
