using Domain.Entites;

namespace Application.Interfaces
{
    public interface IDiplomaRepo
    {
        IQueryable<Diploma> GetAllDiplomasAsync(int Page, int PageSize);
        int GetTotalDiplomasCount();
    }
}
