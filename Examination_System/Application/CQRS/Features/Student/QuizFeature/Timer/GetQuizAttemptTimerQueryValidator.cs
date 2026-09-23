using FluentValidation;

namespace Application.CQRS.Features.Student.QuizFeature.Timer
{
    internal class GetQuizAttemptTimerQueryValidator : AbstractValidator<GetQuizAttemptTimerQuery>
    {
        public GetQuizAttemptTimerQueryValidator()
        {
            RuleFor(x => x.AttemptId)
                .NotEqual(Guid.Empty)
                .WithMessage("Attempt ID is required.");
        }
    }
}