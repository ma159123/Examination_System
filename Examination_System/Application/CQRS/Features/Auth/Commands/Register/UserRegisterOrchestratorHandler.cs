using Application.CQRS.Features.Otp.GenerateOtp;
using Application.CQRS.Features.UserFeature.Commands.CreateUser;
using Application.DTOs;
using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Enums;
using MediatR;

namespace Application.CQRS.Features.Auth.Commands.Register
{
    public class UserRegisterOrchestratorHandler : IRequestHandler<UserRegisterCommand, Result<UserRegistrationResponseDto>>
    {
        private readonly IMediator _mediator;
        private readonly IUserRepo _userRepo;
        public UserRegisterOrchestratorHandler(IMediator mediator, IUserRepo userRepo)
        {
            _mediator = mediator;
            _userRepo = userRepo;
        }
        public async Task<Result<UserRegistrationResponseDto>> Handle(UserRegisterCommand request, CancellationToken cancellationToken)
        {

            var isExist = await _userRepo.isEmailExistAsync(request.Email);
            if (isExist)
            {
                return Result.Failure<UserRegistrationResponseDto>(Error.AlreadyEmailExist);
            }

            string userId;

            // 1. Transaction تقتصر فقط على الداتا بيز (Create User + Add Role)
            using (var transaction = await _userRepo.BeginTransactionAsync(cancellationToken))
            {
                try
                {
                    var userResult = await _mediator.Send(new AddUserCommand
                    (
                        request.Email,
                        request.Password,
                        request.FullName,
                        UserRoles.User
                    ), cancellationToken);

                    if (userResult.IsFailure)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        return Result.Failure<UserRegistrationResponseDto>(error: userResult.Error);
                    }

                    var roleName = UserRoles.User.ToString();
                    var roleResult = await _userRepo.AddToRoleAsync(userResult.Value, roleName);

                    if (!roleResult.Succeeded)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        return Result.Failure<UserRegistrationResponseDto>(
                            new Error("Auth.RoleAssignmentFailed", "Failed to assign role to user.", 400));
                    }

                    await transaction.CommitAsync(cancellationToken);
                    userId = userResult.Value.Id.ToString();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            }

            // 2. send OTP email 
            var sendEmailResult = await _mediator.Send(new GenerateAndSendOtpCommand
            (
                userId,
                request.Email
            ), cancellationToken);

            var response = new UserRegistrationResponseDto
            (
                userId,
                request.Email,
                request.FullName,
                UserStatus.Pending.ToString(),
                DateTime.UtcNow
            );

            string responseMessage = sendEmailResult.IsSuccess
                ? "Registration successful. Please verify your email."
                : "Registration successful, but failed to send verification email. Please use resend OTP option.";

            return Result.Success(data: response, message: responseMessage);
        }
    }
}
