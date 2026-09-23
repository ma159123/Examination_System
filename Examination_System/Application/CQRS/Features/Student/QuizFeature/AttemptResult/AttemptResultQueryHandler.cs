using Application.DTOs;
using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Enums;
using MediatR;

namespace Application.CQRS.Features.Student.QuizFeature.AttemptResult
{
    public class AttemptResultQueryHandler : IRequestHandler<AttemptResultQuery, Result<GetAttemptResultResponse>>
    {
        private readonly IQuizRepo _quizRepo;
        private readonly ICurrentUserService _currentUserService;
        public AttemptResultQueryHandler(IQuizRepo quizRepo, ICurrentUserService currentUserService)
        {
            _quizRepo = quizRepo;
            _currentUserService = currentUserService;
        }
        public async Task<Result<GetAttemptResultResponse>> Handle(AttemptResultQuery request, CancellationToken cancellationToken)
        {
            var attempt = await _quizRepo.GetAttemptWithDetailsAsync(request.AttemptId, cancellationToken);
            if (attempt == null)
            {
                return Result.Failure<GetAttemptResultResponse>(Error.NotFound, "Attempt not found.");
            }
            if (attempt.StudentId != _currentUserService.UserId)
            {
                return Result.Failure<GetAttemptResultResponse>(Error.Forbidden, "You are not allowed to see this attempt.");
            }
            if (attempt.Status == AttemptStatus.InProgress)
            {
                return Result.Failure<GetAttemptResultResponse>(Error.Forbidden, "Result not available until submitted.");
            }


            return Result.Success(new GetAttemptResultResponse
            {
                Score = attempt.Score,
                Passed = attempt.IsPassed,
                TotalQuestions = attempt.Quiz.Questions.Count(),
                CorrectCount = attempt.Answers.Count(a => a.IsCorrect),
                QuestionResultResponses = attempt.Answers.Select(a => new QuestionResultResponse
                {
                    QuestionId = a.QuestionId,
                    CorrectAnswer = a.Question.Options.FirstOrDefault(o => o.IsCorrect)?.Text,
                    StudentAnswer = a.Question.Options.FirstOrDefault(o => o.Id == a.SelectedOptionId)?.Text,
                    IsCorrect = a.IsCorrect,
                }).ToList()
            });
        }
    }
}
