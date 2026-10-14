using Domain.Common;
using MediatR;

namespace Application.CQRS.Features.Student.Diplomas.Commands.DeleteDiploma
{
    public record DeleteDiplomaCommand(Guid DiplomaId) : IRequest<Result>;

}
