using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using MediatR;

namespace Application.CQRS.Features.Auth.Commands.VerifyOTP
{
    public class VerifyOTPCommandHandler : IRequestHandler<VerifyOTPCommand, Result>
    {
        private readonly IOtpService _otpService;
        private readonly IUserRepo _userRepo;
        public VerifyOTPCommandHandler(IOtpService otpService, IUserRepo userRepo)
        {
            _otpService = otpService;
            _userRepo = userRepo;
        }
        public async Task<Result> Handle(VerifyOTPCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepo.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Result.Failure(Error.UserNotFound);

            }
            var result = await _otpService.VerifyOTP(user.Id, request.Otp, cancellationToken);
            if (result.IsFailure)
            {
                return Result.Failure(new Error("Invalid OTP", "The provided OTP is invalid or has expired."));
            }



            //activate user account
            var activateResult = await _userRepo.ActivateUserAsync(user, cancellationToken);

            if (activateResult.IsFailure)
            {
                return Result.Failure(new Error("Activation Failed", "Failed to activate the user account."));

            }
            return Result.Success(message: "User activated successfully.");

        }
    }
}
