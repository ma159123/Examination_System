namespace Application.Interfaces
{
    public interface IEmailService
    {
        Task SendOtpEmailAsync(string email, string otpCode, CancellationToken cancellationToken);
        Task SendEmailTokenAsync(string email, string token, CancellationToken cancellationToken);
    }
}
