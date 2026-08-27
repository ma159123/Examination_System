using Application.CQRS.Features.Otp.GenerateOtp;
using Application.CQRS.Features.UserFeature.Commands.CreateUser;
using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using MediatR;

namespace Application.CQRS.Features.Auth.Commands.Register
{
    public class UserLoginOrchestratorHandler : IRequestHandler<UserRegisterCommand, Result<RegisterResponse>>
    {
        private readonly IMediator _mediator;
        private readonly IUserRepo _userRepo;
        public UserLoginOrchestratorHandler(IMediator mediator, IUserRepo userRepo)
        {
            _mediator = mediator;
            _userRepo = userRepo;
        }
        public async Task<Result<RegisterResponse>> Handle(UserRegisterCommand request, CancellationToken cancellationToken)
        {

            //check if user already exists
            //var checkUserExistsResult = await _mediator.Send(new IsEmailExistQuery
            //(
            //      request.Email
            //), cancellationToken);
            //if (checkUserExistsResult.Value)
            //{
            //    return Result.Failure<RegisterResponse>(Error.AlreadyEmailExist);
            //}
            var isExist = await _userRepo.isEmailExistAsync(request.Email);
            if (isExist)
            {
                return Result.Failure<RegisterResponse>(Error.AlreadyEmailExist);
            }
            //create user
            var createUserResult = await _mediator.Send(new AddUserCommand
            (
                  request.Email,
                  request.Password,
                  request.FullName
            ), cancellationToken);

            if (createUserResult.IsFailure)
            {
                return Result.Failure<RegisterResponse>(createUserResult.Error);
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
                return Result.Failure<RegisterResponse>(sendEmailConfirmationResult.Error);
            }

            var response = new RegisterResponse
            (
                 userId,
                 request.Email,
                 "User registered successfully. Please check your email for confirmation."
            );

            return Result.Success<RegisterResponse>(response);
        }
    }
}
