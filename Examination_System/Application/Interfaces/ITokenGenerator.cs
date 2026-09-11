using Domain.Entites;

namespace Application.Interfaces
{
    public interface ITokenGenerator
    {
        public Task<(string accessToken, DateTime accessTokenExpiry, string refreshToken, DateTime refreshTokenExpiry)>
              GenerateAndSaveTokensAsync(AppUser user, IList<string> roles, CancellationToken cancellationToken = default);
        Task<string> GenerateAndSaveResetTokenAsync(string userId, CancellationToken cancellationToken = default);


        Task<ResetTokenRecord?> ValidateResetTokenAsync(string token, CancellationToken cancellationToken = default);


        Task InvalidateTokenAsync(string token, CancellationToken cancellationToken = default);
        Task<bool> ValidateRefreshTokenAsync(string userId, string refreshToken, CancellationToken cancellationToken = default);

        Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    }
}
