using FluentValidation;

namespace Application.CQRS.Features.Student.QuizFeature.Queries.QuizAttemptHistory
{
    internal class GetStudentAttemptsQueryValidator : AbstractValidator<GetStudentAttemptsQuery>
    {
        public GetStudentAttemptsQueryValidator()
        {
            RuleFor(x => x.DiplomaId)
                .NotEqual(Guid.Empty)
                .WithMessage("Diploma ID is required.");
        }
    }
}
