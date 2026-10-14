using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.CQRS.Features.Student.QuizFeature.Queries.AttemptResult;

public record AttemptResultQuery(Guid AttemptId) : IRequest<Result<GetAttemptResultResponse>>;
