using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.CQRS.Features.Student.QuizFeature.AttemptResult;

public record AttemptResultQuery(Guid AttemptId) : IRequest<Result<GetAttemptResultResponse>>;
