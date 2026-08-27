using Application.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.CQRS.Features.UserFeature.Commands.IsEmailExist
{
    public class IsEmailExistQueryHandler : IRequestHandler<IsEmailExistQuery, Result<bool>>
    {
        private readonly IUserRepo _userRepo;
        public IsEmailExistQueryHandler(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }
        public async Task<Result<bool>> Handle(IsEmailExistQuery request, CancellationToken cancellationToken)
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
