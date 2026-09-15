using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using MediatR;

namespace Application.CQRS.Features.Student.Diplomas.EnrollDiploma
{
    internal class EnrollDiplomaCommandHandler : IRequestHandler<EnrollDiplomaCommand, Result>
    {
        private readonly IDiplomaRepo _diplomaRepo;
        private readonly ICurrentUserService _currentUserService;
        public EnrollDiplomaCommandHandler(IDiplomaRepo diplomaRepo, ICurrentUserService currentUserService)
        {
            _diplomaRepo = diplomaRepo;
            _currentUserService = currentUserService;
        }
        public Task<Result> Handle(EnrollDiplomaCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (userId == null)
            {
                return Task.FromResult(Result.Failure(Error.Unauthorized, "User not authenticated."));
            }

            var diploma = _diplomaRepo.EnrollDiploma(request.DiplomaId, (Guid)userId);

            if (diploma == null)
            {
                return Task.FromResult(Result.Failure(Error.DbError, "Error occurred while enrolling in the diploma."));
            }

            return Task.FromResult(Result.Success(message: "Diploma enrolled successfully."));
        }
    }
}
