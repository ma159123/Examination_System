using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.CQRS.Features.Student.Diplomas.GetAllDiplomas;

public record GetCatalogDiplomasQuery(int Page = 1, int PerPage = 20) : IRequest<Result<PaginatedListResponse<GetDiplomaResponse>>>;
