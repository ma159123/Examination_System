using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.CQRS.Features.Student.Diplomas.GetDiplomaById
{
    public record GetDiplomaByIdQuery(Guid DiplomaId) : IRequest<Result<GetDiplomaResponse>>;
}
