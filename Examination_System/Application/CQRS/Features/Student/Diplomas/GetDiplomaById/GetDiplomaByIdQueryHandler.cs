using Application.CQRS.Mapping.Diplomas;
using Application.DTOs;
using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using MediatR;

namespace Application.CQRS.Features.Student.Diplomas.GetDiplomaById
{
    public class GetDiplomaByIdQueryHandler : IRequestHandler<GetDiplomaByIdQuery, Result<GetDiplomaResponse>>
    {
        private readonly IDiplomaRepo _diplomaRepo;
        private readonly ICurrentUserService _currentUserService;
        public GetDiplomaByIdQueryHandler(IDiplomaRepo diplomaRepo, ICurrentUserService currentUserService)
        {
            _diplomaRepo = diplomaRepo;
            _currentUserService = currentUserService;
        }
        public Task<Result<GetDiplomaResponse>> Handle(GetDiplomaByIdQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (userId == null)
            {
                return Task.FromResult(Result.Failure<GetDiplomaResponse>(Error.Unauthorized, "User not authenticated."));
            }


            var diploma = _diplomaRepo.GetDiplomaById(request.DiplomaId);
            if (diploma == null)
            {
                return Task.FromResult(Result.Failure<GetDiplomaResponse>(Error.NotFound, "Diploma not found."));
            }
            var diplomaResponse = diploma.ToGetDiplomaResponse();

            return Task.FromResult(Result.Success(diplomaResponse));
        }
    }
}
