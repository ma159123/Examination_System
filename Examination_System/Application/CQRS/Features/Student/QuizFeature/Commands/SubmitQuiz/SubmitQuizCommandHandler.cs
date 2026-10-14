using Application.DTOs;
using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Enums;
using MediatR;

namespace Application.CQRS.Features.Student.QuizFeature.Commands.SubmitQuiz
{
    public class SubmitQuizCommandHandler : IRequestHandler<SubmitQuizCommand, Result<SubmitQuizResponse>>
    {
        private readonly IQuizRepo _quizRepo;
        private readonly ICurrentUserService _currentUserService;
        public SubmitQuizCommandHandler(IQuizRepo quizRepo, ICurrentUserService currentUserService)
        {
            _quizRepo = quizRepo;
            _currentUserService = currentUserService;
        }
        public async Task<Result<SubmitQuizResponse>> Handle(SubmitQuizCommand request, CancellationToken cancellationToken)
        {
            var attempt = await _quizRepo.GetAttemptWithDetailsAsync(request.AttemptId, cancellationToken);
            if (attempt == null)
            {
                return Result.Failure<SubmitQuizResponse>(Error.NotFound, "Attempt not found.");
            }
            if (attempt.StudentId != _currentUserService.UserId)
            {
                return Result.Failure<SubmitQuizResponse>(Error.Forbidden, "You are not allowed to submit this attempt.");
            }
            if (attempt.Status != AttemptStatus.InProgress)
            {
                return Result.Failure<SubmitQuizResponse>(Error.Conflict, "This attempt is not in progress.");
            }

            var score = 0;
            foreach (var answer in attempt.Answers)
            {
                if (answer.IsCorrect)
                {
                    score += answer.EarnedPoints;
                }
            }
            bool passed = score >= attempt.Quiz.PassScore;
            await _quizRepo.SubmitAttempt(attempt, score, passed, cancellationToken);

            return Result.Success(new SubmitQuizResponse
            {
                AttemptId = attempt.Id,
                Score = score,
                Passed = passed
            }, "Quiz submitted successfully.");
        }
    }
}
