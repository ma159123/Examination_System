using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.CQRS.Features.Student.QuizFeature.Commands.SubmitQuiz;

public record SubmitQuizCommand(Guid AttemptId) : IRequest<Result<SubmitQuizResponse>>;
