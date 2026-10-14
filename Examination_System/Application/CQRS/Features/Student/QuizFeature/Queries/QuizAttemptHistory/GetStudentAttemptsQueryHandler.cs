using Domain.Common;
using Domain.Common.Errors;
using Application.DTOs;
using Application.Interfaces;
using MediatR;
namespace Application.CQRS.Features.Student.QuizFeature.Queries.QuizAttemptHistory;


public class GetStudentAttemptsQueryHandler
      : IRequestHandler<GetStudentAttemptsQuery, Result<PaginatedListResponse<GetStudentAttemptResponse>>>
{
    private readonly IQuizRepo _quizRepo;
    private readonly ICurrentUserService _currentUserService;

    public GetStudentAttemptsQueryHandler(IQuizRepo quizRepo, ICurrentUserService currentUserService)
    {
        _quizRepo = quizRepo;
        _currentUserService = currentUserService;
    }

    public async Task<Result<PaginatedListResponse<GetStudentAttemptResponse>>> Handle(
        GetStudentAttemptsQuery request,
        CancellationToken cancellationToken)
    {
        var studentId = _currentUserService.UserId;
        if (studentId == null)
        {
            return Result.Failure<PaginatedListResponse<GetStudentAttemptResponse>>(
                Error.Unauthorized,
                "User is not authenticated.");
        }

        var result = await _quizRepo.GetStudentAttemptHistoryAsync(
            studentId.Value,
            request.QuizId,
            request.DiplomaId,
            request.Page,
            request.PerPage,
            cancellationToken);

        return Result.Success(result);
    }
}

