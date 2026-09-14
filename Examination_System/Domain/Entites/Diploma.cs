using Domain.Enums;

namespace Domain.Entites
{
    public class Diploma
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty; // Max: 200 chars
        public string? Description { get; set; }          // Max: 1000 chars
        public ContentStatus Status { get; set; } = ContentStatus.Draft;

        public int QuizCount { get; set; } = 0;
        // Audit & Soft Delete properties
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; } // For soft-delete handling

        // Navigation Property
        public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
    }
}
