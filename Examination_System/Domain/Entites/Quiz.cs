using Domain.Entities;
using Domain.Enums;

namespace Domain.Entites
{
    public class Quiz
    {
        public Guid Id { get; set; }

        // Foreign Key
        public Guid DiplomaId { get; set; }
        public Diploma Diploma { get; set; } = null!;

        public string Title { get; set; } = string.Empty; // Max: 200 chars[cite: 3]
        public string? Instructions { get; set; }
        public int DurationMinutes { get; set; }          // Must be positive
        public int PassScore { get; set; } = 60;          // Range: 0-100, default: 60
        public int? MaxAttempts { get; set; }             // null = unlimited
        public ContentStatus Status { get; set; } = ContentStatus.Draft;

        // Audit Properties
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation Property
        public ICollection<Question> Questions { get; set; } = new List<Question>();
        public ICollection<QuizAttempt> Attempts { get; set; } = new List<QuizAttempt>();
    }
}
