using FluentValidation;

namespace Application.CQRS.Features.Student.QuizFeature.Commands.SubmitQuiz
{
    internal class SubmitQuizCommandValidator : AbstractValidator<SubmitQuizCommand>
    {
        public SubmitQuizCommandValidator()
        {
            RuleFor(x => x.AttemptId)
                .NotEqual(Guid.Empty)
                .WithMessage("Attempt ID is required.");
        }
    }
}