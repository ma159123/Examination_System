using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Entites;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.CQRS.Features.Otp.GenerateOtp
{
    internal class GenerateAndSendOtpCommandHandler : IRequestHandler<GenerateAndSendOtpCommand, Result>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IOtpService _otpService;
        public GenerateAndSendOtpCommandHandler(UserManager<AppUser> userManager, IEmailService emailService, IOtpService otpService)
        {
            _userManager = userManager;
            _emailService = emailService;
            _otpService = otpService;
        }
        public async Task<Result> Handle(GenerateAndSendOtpCommand request, CancellationToken cancellationToken)
        {
            ///get user
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Result.Failure(Error.NotFound);
            }
            // 4 - generate otp 
            var result = await _otpService.GenerateAndStoreOtpAsync(user.Id, cancellationToken);

            // 5. send email with otp
            await _emailService.SendOtpEmailAsync(
                user.Email!,
                result.Value,
                cancellationToken);
            return Result.Success(message:
   "OTP Sent successfully. Please check your email to confirm your account."
);
        }
    }
}
