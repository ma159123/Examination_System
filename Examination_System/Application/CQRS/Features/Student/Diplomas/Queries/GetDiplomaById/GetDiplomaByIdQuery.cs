using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.CQRS.Features.Student.Diplomas.Queries.GetDiplomaById
{
    public record GetDiplomaByIdQuery(Guid DiplomaId) : IRequest<Result<GetDiplomaResponse>>;
}
