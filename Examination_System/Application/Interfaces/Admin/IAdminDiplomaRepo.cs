using Domain.Entites;

namespace Application.Interfaces.Admin
{
    public interface IAdminDiplomaRepo
    {
        Task CreateDiplomaAsync(Diploma diploma, CancellationToken cancellationToken = default);
    }
}
