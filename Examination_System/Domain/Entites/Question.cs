namespace Domain.Entites
{
    public class Question
    {
        public Guid Id { get; set; }

        // Foreign Key
        public Guid QuizId { get; set; }
        public Quiz Quiz { get; set; } = null!;

        public string Text { get; set; } = string.Empty;
        public string? Explanation { get; set; }           // Revealed only post-submission
        public int OrderIndex { get; set; }                 // Controls base display order

        // Audit & Soft Delete
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeletedAt { get; set; }            // Soft-delete

        // Navigation Property
        public ICollection<QuestionOption> Options { get; set; } = new List<QuestionOption>();
    }
}
