using Domain.Common;

namespace Application.Interfaces
{
    public interface IOtpService
    {
        Task<Result<string>> GenerateAndStoreOtpAsync(string userId, CancellationToken cancellationToken);
        Task<Result> VerifyOTP(string Email, string Otp, CancellationToken ct);
    }
}
