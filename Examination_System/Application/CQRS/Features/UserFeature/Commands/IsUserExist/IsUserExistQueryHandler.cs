using Application.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.CQRS.Features.UserFeature.Commands.IsEmailExist
{
    public class IsUserExistQueryHandler : IRequestHandler<IsUserExistQuery, Result<bool>>
    {
        private readonly IUserRepo _userRepo;
        public IsUserExistQueryHandler(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }
        public async Task<Result<bool>> Handle(IsUserExistQuery request, CancellationToken cancellationToken)
        {
            var isUserExist = await _userRepo.FindByEmailAsync(request.Email);
            if (isUserExist != null)
            {
                return Result<bool>.Success(true);
            }

            return Result<bool>.Success(false);
        }
    }
}
