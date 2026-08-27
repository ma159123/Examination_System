using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Enums;
using MediatR;

namespace Application.CQRS.Features.Auth.Commands.ForgotPassword
{
    internal class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result>
    {
        private readonly IUserRepo _userRepo;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IEmailService _emailService;
        public ForgotPasswordCommandHandler(IUserRepo userRepo, ITokenGenerator tokenGenerator, IEmailService emailService)
        {
            _userRepo = userRepo;
            _tokenGenerator = tokenGenerator;
            _emailService = emailService;
        }
        public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            //check if the email exists
            var user = await _userRepo.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Result.Failure(Error.UserNotFound);
            }
            //check if the email is confirmed
            if (!(user.Status == UserStatus.Active))
            {
                return Result.Failure(Error.EmailNotConfirmed);
            }
            //generate reset token and send email logic
            var resetToken = await _tokenGenerator.GenerateAndSaveResetTokenAsync(user.Id, cancellationToken);
            await _emailService.SendEmailTokenAsync(user.Email, resetToken, cancellationToken);

            return Result.Success("Reset token sent successfully.");
        }
    }
}
