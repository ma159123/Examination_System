using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using MediatR;

namespace Application.CQRS.Features.Student.QuizFeature.Timer
{
    internal class GetQuizAttemptTimerQueryHandler : IRequestHandler<GetQuizAttemptTimerQuery, Result<int>>
    {
        private readonly IQuizRepo _quizRepo;
        private readonly ICurrentUserService _currentUserService;
        public GetQuizAttemptTimerQueryHandler(IQuizRepo quizRepo, ICurrentUserService currentUserService)
        {
            _quizRepo = quizRepo;
            _currentUserService = currentUserService;
        }
        public async Task<Result<int>> Handle(GetQuizAttemptTimerQuery request, CancellationToken cancellationToken)
        {
            var studentId = _currentUserService.UserId;
            if (studentId == null)
            {
                return Result.Failure<int>(Error.Unauthorized, "User not authenticated.");
            }
            var timeDetails = await _quizRepo.GetAttemptTimeDetailsAsync(request.AttemptId, cancellationToken);

            if (timeDetails == null)
            {
                return Result.Failure<int>(Error.NotFound, "Attempt not found.");
            }

            var (startedAt, durationMinutes) = timeDetails.Value;

            // 1. calculate the deadline by adding the duration to the startedAt time
            var deadline = startedAt.AddMinutes(durationMinutes);
            var now = DateTime.UtcNow;

            // 2. calculate the remaining time in seconds
            var secondsRemaining = (int)Math.Max(0, (deadline - now).TotalSeconds);

            // 3. if the time has expired (secondsRemaining <= 0), trigger the auto-submit logic
            if (secondsRemaining <= 0)
            {
                // trigger auto-submit logic if attempt is still in_progress
            }

            return Result.Success(secondsRemaining);
        }
    }
}
