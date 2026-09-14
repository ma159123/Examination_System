namespace Domain.Entites
{
    public class QuestionOption
    {
        public Guid Id { get; set; }

        // Foreign Key
        public Guid QuestionId { get; set; }
        public Question Question { get; set; } = null!;

        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }                 // Server-side validation flag[cite: 1, 2, 3]
    }
}
