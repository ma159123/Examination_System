using Application.Interfaces.Admin;
using Domain.Entites;
using Infrastructure.Data;

namespace Infrastructure.Services
{
    public class AdminDiplomaRepo : IAdminDiplomaRepo
    {
        private readonly AppDbContext _context;

        public AdminDiplomaRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateDiplomaAsync(Diploma diploma, CancellationToken cancellationToken = default)
        {
            await _context.Diplomas.AddAsync(diploma, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
