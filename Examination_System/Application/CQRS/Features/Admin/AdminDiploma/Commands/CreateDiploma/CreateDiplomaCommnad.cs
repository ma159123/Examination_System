using Domain.Common;
using MediatR;

namespace Application.CQRS.Features.Admin.AdminDiploma.Commands.CreateDiploma
{
    public record CreateDiplomaCommand(
        string Title,
        string? Description) : IRequest<Result<CreateDiplomaResponse>>;
    public record CreateDiplomaResponse(
        Guid Id,
        string Title,
        string? Description,
        string Status,
        DateTime CreatedAt
    );
}
