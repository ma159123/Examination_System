using Application.CQRS.Features.UserFeature.Commands.CreateUser;
using Application.DTOs;
using Application.Events.UserRegisteredEvent;
using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Entites;
using Domain.Enums;
using MediatR;
using System.Text.Json;

namespace Application.CQRS.Features.Auth.Commands.Register
{
    public class UserRegisterOrchestratorHandler : IRequestHandler<UserRegisterCommand, Result<UserRegistrationResponseDto>>
    {
        private readonly IMediator _mediator;
        private readonly IUserRepo _userRepo;
        private readonly IOutboxRepository _outboxRepo;

        public UserRegisterOrchestratorHandler(
            IMediator mediator,
            IUserRepo userRepo,
            IOutboxRepository outboxRepo)
        {
            _mediator = mediator;
            _userRepo = userRepo;
            _outboxRepo = outboxRepo;
        }

        public async Task<Result<UserRegistrationResponseDto>> Handle(UserRegisterCommand request, CancellationToken cancellationToken)
        {
            var isExist = await _userRepo.isEmailExistAsync(request.Email);
            if (isExist)
            {
                return Result.Failure<UserRegistrationResponseDto>(Error.AlreadyEmailExist);
            }

            string userId;

            // 1. Transaction  (Create User + Add Role + Save Outbox Message) 
            using (var transaction = await _userRepo.BeginTransactionAsync(cancellationToken))
            {
                try
                {
                    // create user
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

                    userId = userResult.Value.Id.ToString();

                    //  Assign role to user
                    var roleName = UserRoles.User.ToString();
                    var roleResult = await _userRepo.AddToRoleAsync(userResult.Value, roleName);

                    if (!roleResult.Succeeded)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        return Result.Failure<UserRegistrationResponseDto>(
                            new Error("Auth.RoleAssignmentFailed", "Failed to assign role to user.", 400));
                    }

                    // add outbox message for generating OTP
                    var domainEvent = new UserRegisteredEvent(userId, request.Email);

                    var outboxMessage = new OutboxMessage
                    {
                        Id = Guid.NewGuid(),
                        Type = domainEvent.GetType().AssemblyQualifiedName!,
                        Payload = JsonSerializer.Serialize(domainEvent),
                        CreatedAt = DateTime.UtcNow
                    };

                    await _outboxRepo.AddAsync(outboxMessage, cancellationToken);
                    await _outboxRepo.SaveChangesAsync(cancellationToken);

                    // 
                    await transaction.CommitAsync(cancellationToken);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
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
                message: "Registration successful. Verification code is being dispatched."
            );
        }
    }
}