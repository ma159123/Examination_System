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
    }
}
