using Domain.Common;
using MediatR;

namespace Application.CQRS.Features.UserFeature.Commands.IsEmailExist;

public record IsEmailExistQuery(string Email) : IRequest<Result<bool>>;
