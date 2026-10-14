using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Enums;
using MediatR;

namespace Application.CQRS.Features.Student.QuizFeature.Commands.AnswerQuestion
{
    public class AnswerQuestionCommandHandler
         : IRequestHandler<AnswerQuestionCommand, Result<AnswerQuestionResponse>>
    {
        private readonly IQuizRepo _quizRepo;
        private readonly ICurrentUserService _currentUserService;

        public AnswerQuestionCommandHandler(IQuizRepo quizRepo, ICurrentUserService currentUserService)
        {
            _quizRepo = quizRepo;
            _currentUserService = currentUserService;
        }

        public async Task<Result<AnswerQuestionResponse>> Handle(
            AnswerQuestionCommand request,
            CancellationToken cancellationToken)
        {
            var studentId = _currentUserService.UserId;
            if (studentId == null)
            {
                return Result.Failure<AnswerQuestionResponse>(Error.Unauthorized, "User is not authenticated.");
            }

            var attempt = await _quizRepo.GetAttemptWithDetailsAsync(request.AttemptId, cancellationToken);

            if (attempt == null)
            {
                return Result.Failure<AnswerQuestionResponse>(Error.NotFound, "Attempt not found.");
            }

            // 1. Student does not own this attempt -> 403 Forbidden
            if (attempt.StudentId != studentId.Value)
            {
                return Result.Failure<AnswerQuestionResponse>(
                    new Error("403", "You do not own this attempt."),
                    "Forbidden");
            }

            // 2. Attempt is already submitted or expired -> 409 Conflict
            if (attempt.Status != AttemptStatus.InProgress)
            {
                return Result.Failure<AnswerQuestionResponse>(
                    new Error("409", "Attempt is already submitted or closed."),
                    "Conflict");
            }

            // 3. Timer has elapsed on server side -> 410 Gone + Trigger Auto-submit
            var deadline = attempt.StartedAt.AddMinutes(attempt.Quiz.DurationMinutes);
            if (DateTime.UtcNow >= deadline)
            {
                await _quizRepo.AutoSubmitAttemptAsync(attempt, cancellationToken);

                return Result.Failure<AnswerQuestionResponse>(
                    new Error("410", "Time limit expired. Attempt auto-submitted."),
                    "Gone");
            }

            // 4. question_id not part of this quiz -> 422 Unprocessable Entity
            var question = attempt.Quiz.Questions.FirstOrDefault(q => q.Id == request.QuestionId);
            if (question == null)
            {
                return Result.Failure<AnswerQuestionResponse>(
                    new Error("422", "Question is not part of this quiz."),
                    "Unprocessable Entity");
            }

            // 5. Verify option exists for this question -> 422 Unprocessable Entity
            var optionExists = question.Options.Any(o => o.Id == request.SelectedOptionId);
            if (!optionExists)
            {
                return Result.Failure<AnswerQuestionResponse>(
                    new Error("422", "Selected option is not valid for this question."),
                    "Unprocessable Entity");
            }

            // 6. Valid attempt_id + valid question_id + valid option_id -> Upsert
            await _quizRepo.UpsertQuestionAnswerAsync(
                request.AttemptId,
                request.QuestionId,
                request.SelectedOptionId,
                cancellationToken);

            return Result.Success(new AnswerQuestionResponse(Saved: true));
        }
    }
}
