using Application.Interfaces;
using Domain.Entites;
using Infrastructure.Data;

namespace Infrastructure.Services
{
    public class DiplomaRepo : IDiplomaRepo
    {
        private readonly AppDbContext _context;
        public DiplomaRepo(AppDbContext context)
        {
            _context = context;
        }
        public IQueryable<Diploma> GetAllDiplomasAsync(int Page, int PageSize)
        {
            return _context.Diplomas.Skip((Page - 1) * PageSize).Take(PageSize);
        }

        public int GetTotalDiplomasCount()
        {
            return _context.Diplomas.Count();
        }
    }
}
