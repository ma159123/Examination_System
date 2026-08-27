using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using MediatR;

namespace Application.CQRS.Features.UserFeature.Commands.CreateUser
{
    public class AddUserCommandHandler : IRequestHandler<AddUserCommand, Result<string>>
    {
        private readonly IUserRepo _userRepo;
        public AddUserCommandHandler(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }
        public async Task<Result<string>> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            //var appUser = request.ToAppUser();
            var userId = await _userRepo.CreatePendingUserAsync(request.Email, request.Password, request.FullName);
            if (string.IsNullOrEmpty(userId))
            {
                return Result.Failure<string>(Error.DbError);
            }
            return Result.Success(userId);
        }
    }
}
