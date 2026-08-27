using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Enums;
using MediatR;

namespace Application.CQRS.Features.Auth.Commands.ActivateAccount;

internal class ActivateAccountCommandHandler : IRequestHandler<ActivateAccountCommand, Result>
{
    private readonly IUserRepo _userRepo;
    private readonly IOtpService _otpService;

    public ActivateAccountCommandHandler(IUserRepo userRepo, IOtpService otpService)
    {
        _userRepo = userRepo;
        _otpService = otpService;
    }

    public async Task<Result> Handle(ActivateAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepo.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Result.Failure(new Error("Auth.InvalidRequest", "Invalid activation request.", 400));
        }

        if (user.EmailConfirmed && user.Status == UserStatus.Active)
        {
            return Result.Failure(new Error("Auth.AlreadyVerified", "Account is already activated.", 400));
        }



        // 1. Verify Hashed OTP (e.g., using BCrypt)
        var result = await _otpService.VerifyOTP(request.Email, request.Otp, cancellationToken);
        if (!result.IsSuccess)
        {
            return Result.Failure(new Error("Auth.InvalidOTP", "Invalid or expired OTP.", 400));
        }
        // 3. Update OTP & User Status
        user.EmailConfirmed = true;
        user.Status = UserStatus.Active;

        var isUpdated = await _userRepo.UpdateAsync(user);
        if (!isUpdated)
        {
            return Result.Failure(new Error("Auth.ActivationFailed", "Failed to activate account.", 500));
        }

        await _userRepo.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
