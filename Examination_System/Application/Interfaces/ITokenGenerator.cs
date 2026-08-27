using Domain.Entites;

namespace Application.Interfaces
{
    public interface ITokenGenerator
    {
        (string accessToken, string refreshToken) GenerateTokens(AppUser user, IList<string> roles);
        Task<string> GenerateAndSaveResetTokenAsync(string userId, CancellationToken cancellationToken = default);


        Task<ResetTokenRecord?> ValidateResetTokenAsync(string token, CancellationToken cancellationToken = default);


        Task InvalidateTokenAsync(string token, CancellationToken cancellationToken = default);
    }
}
