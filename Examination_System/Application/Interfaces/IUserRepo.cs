using Domain.Common;
using Domain.Entites;

namespace Application.Interfaces
{
    public interface IUserRepo
    {
        Task<AppUser?> FindByEmailAsync(string email);
        Task<bool> isEmailExistAsync(string email);
        Task<bool> UpdateAsync(AppUser user);
        Task<string?> CreatePendingUserAsync(string email, string password, string fullName);

        Task<bool?> CheckPasswordAsync(AppUser user, string password);
        Task<bool> ChangePasswordAsync(AppUser user, string currentPassword, string NewPassword);
        public Task<Result> ResetPasswordAsync(AppUser user, string newPassword, CancellationToken ct);
        Task<AppUser?> FindByIdAsync(string userId);
        Task<Result> ActivateUserAsync(AppUser user, CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct);
    }
}
