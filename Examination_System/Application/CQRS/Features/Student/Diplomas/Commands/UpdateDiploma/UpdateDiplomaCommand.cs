using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.CQRS.Features.Student.Diplomas.Commands.UpdateDiploma
{
    public record UpdateDiplomaCommand(Guid diplomaId, string Title, string? Description) : IRequest<Result<GetDiplomaResponse>>;
}
