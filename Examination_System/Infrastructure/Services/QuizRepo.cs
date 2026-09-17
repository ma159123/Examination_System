using Application.DTOs;
using Application.Interfaces;
using Domain.Entites;
using Domain.Entities;
using Domain.Enums;
using global::Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Services;


public class QuizRepo : IQuizRepo
{
    private readonly AppDbContext _context;

    public QuizRepo(AppDbContext context)
    {
        _context = context;
    }

    public QuizAttempt CreateQuizAttemptAsync(Quiz quiz, Guid studentId, CancellationToken cancellationToken)
    {
        var startTime = DateTime.UtcNow;
        var deadline = startTime.AddMinutes(quiz.DurationMinutes);

        var newAttempt = new QuizAttempt
        {
            Id = Guid.NewGuid(),
            QuizId = quiz.Id,
            StudentId = studentId,
            StartedAt = startTime,
            Status = AttemptStatus.InProgress
        };
        _context.QuizAttempts.Add(newAttempt);
        return newAttempt;
    }

    public Task<QuizAttempt?> GetInProgressAttempt(Guid quizId, Guid studentId, CancellationToken cancellationToken)
    {
        return _context.QuizAttempts
                 .FirstOrDefaultAsync(a => a.QuizId == quizId
                                        && a.StudentId == studentId
                                        && a.Status == AttemptStatus.InProgress, cancellationToken);
    }

    public async Task<Quiz?> GetQuizAsync(Guid quizId, CancellationToken cancellationToken)
    {
        return await _context.Quizzes
            .AsNoTracking()
            .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(q => q.Id == quizId && q.Status == ContentStatus.Published, cancellationToken);
    }
    public async Task<PaginatedListResponse<GetStudentAttemptResponse>> GetStudentAttemptHistoryAsync(
        Guid studentId,
        Guid? quizId,
        Guid? diplomaId,
        int page,
        int perPage,
        CancellationToken cancellationToken = default)
    {

        var query = _context.QuizAttempts
            .AsNoTracking()
            .Where(a => a.StudentId == studentId);


        if (quizId.HasValue && quizId.Value != Guid.Empty)
        {
            query = query.Where(a => a.QuizId == quizId.Value);
        }

        if (diplomaId.HasValue && diplomaId.Value != Guid.Empty)
        {
            query = query.Where(a => a.Quiz.DiplomaId == diplomaId.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);


        var items = await query
            .OrderByDescending(a => a.SubmittedAt)
            .Select(a => new GetStudentAttemptResponse
            {
                AttemptId = a.Id,
                QuizId = a.QuizId,
                QuizTitle = a.Quiz.Title,
                DiplomaId = a.Quiz.DiplomaId,
                DiplomaTitle = a.Quiz.Diploma.Title,
                Score = a.Score,
                IsPassed = a.IsPassed,
                StartedAt = a.StartedAt,
                SubmittedAt = a.SubmittedAt
            })
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(cancellationToken);

        return new PaginatedListResponse<GetStudentAttemptResponse>(items, totalCount, page, perPage);
    }

    public async Task<bool> IsMaxAttemptReached(Quiz quiz, Guid studentId, CancellationToken cancellationToken)
    {
        if (quiz.MaxAttempts.HasValue)
        {
            var completedAttemptsCount = await _context.QuizAttempts
                .CountAsync(a => a.QuizId == quiz.Id && a.StudentId == studentId, cancellationToken);

            if (completedAttemptsCount >= quiz.MaxAttempts.Value)
            {
                return true;
            }
        }
        return false;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public List<QuestionDto> ShuffleQuestions(List<Question> questions)
    {
        return questions
                  .OrderBy(_ => Random.Shared.Next())
                  .Select((q, index) => new QuestionDto
                  {
                      QuestionId = q.Id,
                      Order = index + 1,
                      Text = q.Text,
                      Options = q.Options
                          .OrderBy(_ => Random.Shared.Next())
                          .Select(o => new OptionDto
                          {
                              OptionId = o.Id,
                              Text = o.Text
                          }).ToList()
                  }).ToList();
    }
}