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


    public async Task<QuizAttempt?> GetAttemptWithDetailsAsync(Guid attemptId, CancellationToken cancellationToken = default)
    {
        return await _context.QuizAttempts
            .Include(a => a.Quiz)
                .ThenInclude(q => q.Questions)
                    .ThenInclude(q => q.Options)
            .Include(a => a.Answers)
            .FirstOrDefaultAsync(a => a.Id == attemptId, cancellationToken);
    }

    public async Task UpsertQuestionAnswerAsync(
    Guid attemptId,
    Guid questionId,
    Guid selectedOptionId,
    CancellationToken cancellationToken = default)
    {
        // 1. get the selected option and check if it's correct
        var option = await _context.QuestionOptions
            .Include(o => o.Question)
            .FirstOrDefaultAsync(o => o.Id == selectedOptionId && o.QuestionId == questionId, cancellationToken);

        bool isCorrect = option?.IsCorrect ?? false;
        int points = isCorrect ? (option?.Question?.Points ?? 1) : 0;

        // 2. search for existing answer for the same question in the same attempt
        var existingAnswer = await _context.QuestionAnswers
            .FirstOrDefaultAsync(qa => qa.QuizAttemptId == attemptId && qa.QuestionId == questionId, cancellationToken);

        if (existingAnswer != null)
        {
            // Update (Overwrite)
            existingAnswer.SelectedOptionId = selectedOptionId;
            existingAnswer.IsCorrect = isCorrect;
            existingAnswer.EarnedPoints = points;
        }
        else
        {
            // Insert
            var newAnswer = new QuestionAnswer
            {
                Id = Guid.NewGuid(),
                QuizAttemptId = attemptId,
                QuestionId = questionId,
                SelectedOptionId = selectedOptionId,
                IsCorrect = isCorrect,
                EarnedPoints = points
            };
            _context.QuestionAnswers.Add(newAnswer);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AutoSubmitAttemptAsync(QuizAttempt attempt, CancellationToken cancellationToken = default)
    {
        attempt.Status = AttemptStatus.TimedOut;
        attempt.SubmittedAt = attempt.StartedAt.AddMinutes(attempt.Quiz.DurationMinutes);

        // Calculate score based only on answers received before/at deadline
        int correctAnswers = 0;
        int totalQuestions = attempt.Quiz.Questions.Count;

        foreach (var answer in attempt.Answers)
        {
            var question = attempt.Quiz.Questions.FirstOrDefault(q => q.Id == answer.QuestionId);
            var selectedOption = question?.Options.FirstOrDefault(o => o.Id == answer.SelectedOptionId);

            if (selectedOption != null && selectedOption.IsCorrect)
            {
                answer.IsCorrect = true;
                answer.EarnedPoints = question!.Points;
                correctAnswers++;
            }
        }

        attempt.Score = totalQuestions > 0 ? (int)Math.Round((double)correctAnswers / totalQuestions * 100) : 0;
        attempt.IsPassed = attempt.Score >= attempt.Quiz.PassScore;

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<(DateTime StartedAt, int DurationMinutes)?> GetAttemptTimeDetailsAsync(
    Guid attemptId,
    CancellationToken cancellationToken = default)
    {
        var details = await _context.QuizAttempts
            .AsNoTracking()
            .Where(a => a.Id == attemptId)
            .Select(a => new { a.StartedAt, a.Quiz.DurationMinutes })
            .FirstOrDefaultAsync(cancellationToken);

        if (details == null) return null;

        return (details.StartedAt, details.DurationMinutes);
    }

    public Task SubmitAttempt(QuizAttempt attempt, int score, bool passed, CancellationToken cancellationToken = default)
    {
        attempt.Status = AttemptStatus.Submitted;
        attempt.Score = score;
        attempt.IsPassed = passed;
        _context.Entry(attempt).State = EntityState.Modified;
        return _context.SaveChangesAsync(cancellationToken);
    }

    //-----------------admin-----------//
    public async Task AddAsync(Quiz quiz, CancellationToken cancellationToken = default)
    {
        await _context.Quizzes.AddAsync(quiz, cancellationToken);
    }
}