using Domain.Entites;

namespace Application.Interfaces
{
    public interface IAdminQuizRepo
    {
        Task<Diploma> CreateDiplomaAsync(Diploma diploma);
    }
}
