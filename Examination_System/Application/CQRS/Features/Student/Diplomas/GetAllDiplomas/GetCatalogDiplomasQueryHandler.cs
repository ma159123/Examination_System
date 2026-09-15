using Application.DTOs;
using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using MediatR;

namespace Application.CQRS.Features.Student.Diplomas.GetAllDiplomas
{
    public class GetCatalogDiplomasQueryHandler : IRequestHandler<GetCatalogDiplomasQuery, Result<PaginatedListResponse<GetDiplomaResponse>>>
    {
        private readonly IDiplomaRepo _diplomaRepo;
        private readonly ICurrentUserService _currentUserService;
        public GetCatalogDiplomasQueryHandler(IDiplomaRepo diplomaRepo, ICurrentUserService currentUserService)
        {
            _diplomaRepo = diplomaRepo;
            _currentUserService = currentUserService;
        }
        public Task<Result<PaginatedListResponse<GetDiplomaResponse>>> Handle(GetCatalogDiplomasQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (userId == null)
            {
                return Task.FromResult(Result.Failure<PaginatedListResponse<GetDiplomaResponse>>(Error.Unauthorized, "User not authenticated."));
            }

            var diplomas = _diplomaRepo.GetStudentDiplomas(request.Page, request.PerPage, (Guid)userId);

            int TotalCount = _diplomaRepo.GetTotalDiplomasCount();
            PaginatedListResponse<GetDiplomaResponse> paginatedListResponse = new PaginatedListResponse<GetDiplomaResponse>(
           diplomas,
           TotalCount,
           request.Page,
           request.PerPage
           );
            return Task.FromResult(Result.Success(paginatedListResponse));
        }
    }
}
