using FluentValidation;

namespace Application.CQRS.Features.Student.QuizFeature.Commands.CreateQuiz
{
    public class CreateQuizCommandValidator : AbstractValidator<CreateQuizCommand>
    {
        public CreateQuizCommandValidator()
        {
            RuleFor(x => x.diplomaId)
                .NotEmpty().WithMessage("Diploma ID is required.");
            RuleFor(x => x.title)
                .NotEmpty().WithMessage("Title is required.")
                .Length(3, 200).WithMessage("Title cannot be less than 3 or exceed 200 characters.");
            RuleFor(x => x.durationMinutes)
                .GreaterThan(0).WithMessage("Duration must be greater than 0 minutes.");
            RuleFor(x => x.passScore)
                .InclusiveBetween(0, 100).WithMessage("Pass score must be between 0 and 100.");
            RuleFor(x => x.maxAttempts)
                .GreaterThanOrEqualTo(0).WithMessage("Max attempts must be a non-negative integer.");
        }
    }
}
