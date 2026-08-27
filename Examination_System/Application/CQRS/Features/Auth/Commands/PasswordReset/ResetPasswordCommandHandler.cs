using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using MediatR;

namespace Application.CQRS.Features.Auth.Commands.PasswordReset
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
    {
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IUserRepo _userRepo;
        public ResetPasswordCommandHandler(ITokenGenerator tokenGenerator, IUserRepo userRepo)
        {
            _tokenGenerator = tokenGenerator;
            _userRepo = userRepo;
        }
        public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            //validate token
            var tokenRecord = await _tokenGenerator.ValidateResetTokenAsync(request.token, cancellationToken);
            if (tokenRecord == null)
            {
                return Result.Failure(Error.InValidTokenError);
            }
            //get user
            var user = await _userRepo.FindByIdAsync(tokenRecord.UserId);
            if (user == null)
            {
                return Result.Failure(Error.UserNotFound);
            }
            var result = await _userRepo.ResetPasswordAsync(user, request.NewPassword, cancellationToken);
            if (!result.IsSuccess)
            {
                return Result.Failure(result.Error);
            }
            tokenRecord.IsUsed = true;
            await _userRepo.SaveChangesAsync(cancellationToken);
            return result;

        }
    }
}
