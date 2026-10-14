using Domain.Common;
using MediatR;

namespace Application.CQRS.Features.Student.Diplomas.Commands.EnrollDiploma
{
    public record EnrollDiplomaCommand(Guid DiplomaId) : IRequest<Result>;
}
