using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailTokenAsync(string email, string token, CancellationToken cancellationToken)
    {
        var smtpHost = _config["Smtp:Host"] ?? "localhost";
        var smtpPort = int.Parse(_config["Smtp:Port"] ?? "25");
        var senderEmail = _config["Smtp:SenderEmail"] ?? "no-reply@examination.com";

        using var client = new SmtpClient(smtpHost, smtpPort)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(
                _config["Smtp:Username"],
                _config["Smtp:Password"]
            )
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(senderEmail, "Examination System"),
            Subject = "Your Account Reset Token",
            Body = $"<h2>Reset Token</h2><p>Your Reset Token is: <strong>{token}</strong></p><p>This token expires in 15 minutes.</p>",
            IsBodyHtml = true
        };

        mailMessage.To.Add(email);

        await client.SendMailAsync(mailMessage, cancellationToken);
    }

    public async Task SendOtpEmailAsync(string email, string otpCode, CancellationToken cancellationToken)
    {
        var smtpHost = _config["Smtp:Host"] ?? "localhost";
        var smtpPort = int.Parse(_config["Smtp:Port"] ?? "25");
        var senderEmail = _config["Smtp:SenderEmail"] ?? "no-reply@examination.com";

        using var client = new SmtpClient(smtpHost, smtpPort)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(
                _config["Smtp:Username"],
                _config["Smtp:Password"]
            )
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(senderEmail, "Examination System"),
            Subject = "Your Account Verification Code",
            Body = $"<h2>Verification Code</h2><p>Your OTP code is: <strong>{otpCode}</strong></p><p>This code expires in 10 minutes.</p>",
            IsBodyHtml = true
        };

        mailMessage.To.Add(email);

        await client.SendMailAsync(mailMessage, cancellationToken);
    }
}