using Domain.Common;
using MediatR;

namespace Application.CQRS.Features.UserFeature.Commands.IsEmailExist;

public record IsUserExistQuery(string Email) : IRequest<Result<bool>>;
