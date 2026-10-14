using Application.DTOs;
using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Entites;
using MediatR;


namespace Application.CQRS.Features.Student.QuizFeature.Commands.StartQuiz
{
    public class StartQuizCommandHandler : IRequestHandler<StartQuizCommand, Result<StartQuizResponse>>
    {
        private readonly IQuizRepo quizRepo;
        private readonly ICurrentUserService _currentUserService;

        public StartQuizCommandHandler(IQuizRepo quizRepo, ICurrentUserService currentUserService)
        {
            this.quizRepo = quizRepo;
            _currentUserService = currentUserService;
        }

        public async Task<Result<StartQuizResponse>> Handle(StartQuizCommand request, CancellationToken cancellationToken)
        {
            var studentId = _currentUserService.UserId;
            if (studentId == null)
            {
                return Result.Failure<StartQuizResponse>(Error.Unauthorized, "User not authenticated.");
            }

            // 1. check if quiz if exist and Published
            var quiz = await quizRepo.GetQuizAsync(request.QuizId, cancellationToken);

            if (quiz == null)
            {
                return Result.Failure<StartQuizResponse>(Error.NotFound, "Quiz not found or not published.");
            }

            // 2. check if the student has an in-progress attempt for this quiz
            var existingAttempt = await quizRepo.GetInProgressAttempt(request.QuizId, studentId.Value, cancellationToken);

            if (existingAttempt != null)
            {
                return Result.Failure<StartQuizResponse>(
                  Error.AlreadyQuizAttemptInProgress,
                    "In-progress attempt exists.");
            }

            // 3.check if the student has reached the maximum allowed attempts for this quiz
            var isMaxAttemptReached = await quizRepo.IsMaxAttemptReached((Quiz)quiz, studentId.Value, cancellationToken);
            if (isMaxAttemptReached)
            {
                return Result.Failure<StartQuizResponse>(Error.Forbidden, "You have reached the maximum allowed attempts for this quiz.");
            }


            // 4. create new attempt and shuffle questions and options
            var newAttempt = quizRepo.CreateQuizAttemptAsync(quiz, studentId.Value, cancellationToken);

            // shuffle questions and options
            var shuffledQuestions = quizRepo.ShuffleQuestions(quiz.Questions.ToList());


            await quizRepo.SaveChangesAsync(cancellationToken);

            var response = new StartQuizResponse
            {
                AttemptId = newAttempt.Id,
                QuizId = quiz.Id,
                QuizTitle = quiz.Title,
                DurationMinutes = quiz.DurationMinutes,
                StartTime = newAttempt.StartedAt,
                Deadline = newAttempt.StartedAt.AddMinutes(quiz.DurationMinutes),
                TotalQuestions = shuffledQuestions.Count,
                Questions = shuffledQuestions
            };

            return Result.Success(response);
        }
    }
}
