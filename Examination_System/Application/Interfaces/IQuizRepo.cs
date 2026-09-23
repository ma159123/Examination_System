using Application.DTOs;
using Domain.Entites;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IQuizRepo
    {

        Task<Quiz?> GetQuizAsync(Guid quizId, CancellationToken cancellationToken);
        Task<PaginatedListResponse<GetStudentAttemptResponse>> GetStudentAttemptHistoryAsync(
            Guid studentId,
            Guid? quizId,
            Guid? diplomaId,
            int page,
            int perPage,
            CancellationToken cancellationToken = default);
        public Task<QuizAttempt?> GetInProgressAttempt(Guid quizId, Guid studentId, CancellationToken cancellationToken);
        Task<bool> IsMaxAttemptReached(Quiz quiz, Guid studentId, CancellationToken cancellationToken);
        public QuizAttempt CreateQuizAttemptAsync(Quiz quiz, Guid studentId, CancellationToken cancellationToken);
        public List<QuestionDto> ShuffleQuestions(List<Question> questions);
        public Task SaveChangesAsync(CancellationToken cancellationToken);
        Task<QuizAttempt?> GetAttemptWithDetailsAsync(Guid attemptId, CancellationToken cancellationToken = default);
        public Task<(DateTime StartedAt, int DurationMinutes)?> GetAttemptTimeDetailsAsync(
           Guid attemptId,
           CancellationToken cancellationToken = default);
        Task UpsertQuestionAnswerAsync(Guid attemptId, Guid questionId, Guid selectedOptionId, CancellationToken cancellationToken = default);
        Task AutoSubmitAttemptAsync(QuizAttempt attempt, CancellationToken cancellationToken = default);
        Task SubmitAttempt(QuizAttempt attempt, int score, bool passed, CancellationToken cancellationToken = default);
    }
}
