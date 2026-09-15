using Application.DTOs;
using Domain.Entites;

namespace Application.Interfaces
{
    public interface IDiplomaRepo
    {
        IQueryable<Diploma> GetAllDiplomasAsync(int Page, int PageSize);
        IQueryable<GetDiplomaResponse> GetStudentDiplomas(int Page, int PageSize, Guid UserId);

        public IQueryable<GetQuizResponse> GetDiplomasQuizes(Guid DiplomaId, Guid studentId);

        Diploma? GetDiplomaById(Guid DiplomaId);
        public Task<bool> IsDiplomaExistsAsync(Guid diplomaId, CancellationToken cancellationToken);
        public Task<bool> IsDiplomaEnrolledAsync(Guid diplomaId, Guid studentId, CancellationToken cancellationToken);

        StudentDiploma? EnrollDiploma(Guid diplomaId, Guid studentId);
        int GetTotalDiplomasCount();
    }
}
