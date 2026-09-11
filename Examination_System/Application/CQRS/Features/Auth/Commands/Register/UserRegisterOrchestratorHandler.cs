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
            //create user
            var createUserResult = await _mediator.Send(new AddUserCommand
            (
                  request.Email,
                  request.Password,
                  request.FullName,
                  UserRoles.User.ToString()

            ), cancellationToken);

            if (createUserResult.IsFailure)
            {
                return Result.Failure<UserRegistrationResponseDto>(error: createUserResult.Error);
            }

            var userId = createUserResult.Value;

            //send email confirmation
            var sendEmailConfirmationResult = await _mediator.Send(new GenerateAndSendOtpCommand
            (
                  userId,
                  request.Email
            ), cancellationToken);

            if (sendEmailConfirmationResult.IsFailure)
            {
                return Result.Failure<UserRegistrationResponseDto>(sendEmailConfirmationResult.Error);
            }

            var response = new UserRegistrationResponseDto
            (
                 userId,
                 request.Email,
                 request.FullName,
                 UserStatus.Pending.ToString(),
                 DateTime.UtcNow
            );

            return Result.Success(
                    data: response,
                    message: "Registration successful. Please verify your email."
                );
        }
    }
}
