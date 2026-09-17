using Domain.Entites;
using Domain.Enums;

namespace Domain.Entities
{
    public class QuizAttempt
    {
        public Guid Id { get; set; }


        public Guid StudentId { get; set; }
        public AppUser Student { get; set; } = null!;

        public Guid QuizId { get; set; }
        public Quiz Quiz { get; set; } = null!;

        public int Score { get; set; } = 0;
        public bool IsPassed { get; set; } = false;

        public AttemptStatus Status { get; set; } = AttemptStatus.InProgress;
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? SubmittedAt { get; set; }

        public ICollection<QuestionAnswer> Answers { get; set; } = new List<QuestionAnswer>();
    }
}