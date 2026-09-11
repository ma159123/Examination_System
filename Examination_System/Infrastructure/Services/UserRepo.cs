using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Entites;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.InterfacesImpl
{
    public class UserRepo : IUserRepo
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _dbContext;
        public UserRepo(UserManager<AppUser> userManager, AppDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task<AppUser?> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<string?> CreatePendingUserAsync(string email, string password, string fullName)
        {

            var user = new AppUser
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                Status = UserStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to create user: {errors}");
            }

            return user.Id;
        }

        public async Task<bool?> CheckPasswordAsync(AppUser user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<bool> ChangePasswordAsync(AppUser user, string currentPassword, string NewPassword)
        {
            var result = await _userManager.ChangePasswordAsync(user, currentPassword, NewPassword);
            return result.Succeeded;
        }

        public async Task<Result> ResetPasswordAsync(AppUser user, string newPassword, CancellationToken ct)
        {
            // 2. تعيين كلمة المرور الجديدة
            var removeResult = await _userManager.RemovePasswordAsync(user);
            if (!removeResult.Succeeded)
            {
                return Result.Failure(new Error("Auth.ResetFailed", "Failed to reset password.", 500));
            }

            var addResult = await _userManager.AddPasswordAsync(user, newPassword);
            if (!addResult.Succeeded)
            {
                var errors = string.Join(", ", addResult.Errors.Select(e => e.Description));
                return Result.Failure(new Error("Auth.ResetFailed", errors, 400));
            }

            return Result.Success(message: "Password reset successfully.");
        }

        public Task<AppUser?> FindByIdAsync(string userId)
        {
            return _userManager.FindByIdAsync(userId);
        }

        public async Task<bool> UpdateAsync(AppUser user)
        {
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task SaveChangesAsync(CancellationToken ct)
        {
            await _dbContext.SaveChangesAsync(ct);
        }

        public async Task<Result> ActivateUserAsync(AppUser user, CancellationToken ct)
        {
            if (user == null)
            {
                return Result.Failure(Error.UserNotFound);
            }

            if (user.EmailConfirmed && user.Status == UserStatus.Active)
            {
                return Result.Failure(new Error("Auth.AlreadyActive", "Account is already activated.", 400));
            }

            user.Status = UserStatus.Active;
            user.EmailConfirmed = true;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return Result.Failure(new Error("Auth.ActivationFailed", "Failed to update user activation status.", 500));
            }

            return Result.Success(message: "User activated successfully.");
        }

        public Task<bool> isEmailExistAsync(string email)
        {
            return _userManager.Users.AnyAsync(u => u.Email == email);
        }
    }
}
