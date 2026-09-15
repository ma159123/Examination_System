using Application.DTOs;
using Application.Interfaces;
using Domain.Entites;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class DiplomaRepo : IDiplomaRepo
    {
        private readonly AppDbContext _context;
        public DiplomaRepo(AppDbContext context)
        {
            _context = context;
        }

        public StudentDiploma? EnrollDiploma(Guid diplomaId, Guid studentId)
        {
            var result = _context.StudentDiplomas.Add(new StudentDiploma
            {
                DiplomaId = diplomaId,
                StudentId = studentId,
            });

            _context.SaveChangesAsync();
            return result.Entity;
        }

        public IQueryable<Diploma> GetAllDiplomasAsync(int Page, int PageSize)
        {
            return _context.Diplomas.Skip((Page - 1) * PageSize).Take(PageSize);
        }

        public Diploma? GetDiplomaById(Guid DiplomaId)
        {
            return _context.Diplomas.FirstOrDefault(d => d.Id == DiplomaId);
        }

        public IQueryable<GetQuizResponse> GetDiplomasQuizes(Guid DiplomaId, Guid studentId)
        {
            return _context.Quizzes
                .Where(q => q.DiplomaId == DiplomaId)
               .Select(q => new GetQuizResponse
               {
                   Id = q.Id,
                   Title = q.Title,
                   DurationMinutes = q.DurationMinutes,
                   PassScore = q.PassScore,
                   MaxAttempts = q.MaxAttempts,

                   // Count attempts for the current student
                   AttemptCount = q.Attempts.Count(a => a.StudentId == studentId),

                   // Get the latest score from the most recent submitted attempt
                   LastScore = q.Attempts
            .Where(a => a.StudentId == studentId && a.SubmittedAt != null)
            .OrderByDescending(a => a.SubmittedAt)
            .Select(a => (int?)a.Score)
            .FirstOrDefault(),

                   // Determine quiz attempt status
                   Status = !q.Attempts.Any(a => a.StudentId == studentId) ? QuizAttemptStatus.NotStarted
            : q.Attempts.Any(a => a.StudentId == studentId && a.IsPassed)
                ? QuizAttemptStatus.Passed
                : QuizAttemptStatus.Failed,

                   // Handle null MaxAttempts (unlimited attempts) or check if student is below limit
                   CanAttempt = (q.MaxAttempts == null || q.Attempts.Count(a => a.StudentId == studentId) < q.MaxAttempts) &&
                     !q.Attempts.Any(a => a.StudentId == studentId && a.IsPassed)
               });
        }


        public IQueryable<GetDiplomaResponse> GetStudentDiplomas(int Page, int PageSize, Guid UserId)
        {
            return _context.StudentDiplomas
                .Where(sd => sd.StudentId == UserId)
                .Select(sd => new GetDiplomaResponse
                {
                    Id = sd.Diploma.Id,
                    Title = sd.Diploma.Title,
                    Description = sd.Diploma.Description,
                    QuizCount = sd.Diploma.Quizzes.Count(),
                    StudentProgress = sd.ProgressPercentage,
                    Status = sd.Diploma.Status,
                    IsEnrolled = true
                })
                .Skip((Page - 1) * PageSize)
                .Take(PageSize);
        }

        public int GetTotalDiplomasCount()
        {
            return _context.Diplomas.Count();
        }

        public Task<bool> IsDiplomaEnrolledAsync(Guid diplomaId, Guid studentId, CancellationToken cancellationToken)
        {
            _context.StudentDiplomas
                 .AsNoTracking()
                 .AnyAsync(sd => sd.DiplomaId == diplomaId && sd.StudentId == studentId, cancellationToken);
            return Task.FromResult(true);
        }

        public async Task<bool> IsDiplomaExistsAsync(Guid diplomaId, CancellationToken cancellationToken)
        {
            var diploma = await _context.Diplomas
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == diplomaId && d.Status == ContentStatus.Published, cancellationToken);

            return diploma != null;
        }
    }
}
