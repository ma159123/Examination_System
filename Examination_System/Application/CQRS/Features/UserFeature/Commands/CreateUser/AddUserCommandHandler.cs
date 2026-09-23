using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Entites;
using MediatR;

namespace Application.CQRS.Features.UserFeature.Commands.CreateUser
{
    public class AddUserCommandHandler : IRequestHandler<AddUserCommand, Result<AppUser>>
    {
        private readonly IUserRepo _userRepo;
        public AddUserCommandHandler(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }
        public async Task<Result<AppUser>> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            //var appUser = request.ToAppUser();
            var user = await _userRepo.CreatePendingUserAsync(request.Email, request.Password, request.FullName);
            if (user == null)
            {
                return Result.Failure<AppUser>(Error.DbError);
            }
            return Result.Success<AppUser>(data: user);
        }
    }
}
