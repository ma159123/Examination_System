using Application.CQRS.Features.Otp.GenerateOtp;
using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using MediatR;

namespace Application.CQRS.Features.Auth.Commands.ResendOTP
{
    public class ResendOTPCommandHandler : IRequestHandler<ResendOTPCommand, Result>
    {
        private readonly IUserRepo _userRepo;
        private readonly IOtpService _otpService;
        private readonly IMediator _mediator;
        public ResendOTPCommandHandler(IUserRepo userRepo, IOtpService otpService, IMediator mediator)
        {
            _userRepo = userRepo;
            _otpService = otpService;
            _mediator = mediator;
        }
        public async Task<Result> Handle(ResendOTPCommand request, CancellationToken cancellationToken)
        {
            //check if user exist and is not authintecated
            var user = await _userRepo.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Result.Failure(Error.UserNotFound);
            }
            else if (user.Status == Domain.Enums.UserStatus.Active)
            {
                return Result.Failure(Error.UserAlreadyAuthenticated);
            }
            //generate otp
            //var result = await _otpService.GenerateAndStoreOtpAsync(user.Id, cancellationToken);
            //if (result.IsFailure)
            //{
            //    return result;
            //}
            //send email with otp
            return await _mediator.Send(new GenerateAndSendOtpCommand
              (
                   user.Id,
                   user.Email
              ), cancellationToken);
        }
    }
}