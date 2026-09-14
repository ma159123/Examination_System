using Application.DTOs;
using MediatR;

namespace Application.CQRS.Features.Student.Diplomas;

public record GetCatalogDiplomasQuery(int Page = 1, int PerPage = 20) : IRequest<PaginatedListResponse<GetDiplomaResponse>>;
