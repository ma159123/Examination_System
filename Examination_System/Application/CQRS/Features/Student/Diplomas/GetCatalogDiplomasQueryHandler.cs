using Application.DTOs;
using Application.Interfaces;
using MediatR;

namespace Application.CQRS.Features.Student.Diplomas
{
    public class GetCatalogDiplomasQueryHandler : IRequestHandler<GetCatalogDiplomasQuery, PaginatedListResponse<GetDiplomaResponse>>
    {
        private readonly IDiplomaRepo _diplomaRepo;
        public GetCatalogDiplomasQueryHandler(IDiplomaRepo diplomaRepo)
        {
            _diplomaRepo = diplomaRepo;
        }
        public Task<PaginatedListResponse<GetDiplomaResponse>> Handle(GetCatalogDiplomasQuery request, CancellationToken cancellationToken)
        {
            var diplomas = _diplomaRepo.GetAllDiplomasAsync(request.Page, request.PerPage);
            List<GetDiplomaResponse> diplomaResponses = new();
            foreach (var diploma in diplomas)
            {
                GetDiplomaResponse diplomaResponse = new GetDiplomaResponse
                {
                    Description = diploma.Description,
                    Id = diploma.Id,
                    IsEnrolled = false,
                    QuizCount = diploma.QuizCount,
                    Status = diploma.Status,
                    StudentProgress = 0,
                    Title = diploma.Title
                };
                diplomaResponses.Add(diplomaResponse);
            }
            int TotalCount = _diplomaRepo.GetTotalDiplomasCount();
            PaginatedListResponse<GetDiplomaResponse> paginatedListResponse = new PaginatedListResponse<GetDiplomaResponse>(
           diplomaResponses,
           TotalCount,
           request.Page,
           request.PerPage
           );
            return Task.FromResult(paginatedListResponse);
        }
    }
}
