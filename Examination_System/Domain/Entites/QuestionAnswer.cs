using Domain.Entites;

namespace Domain.Entities
{
    public class QuestionAnswer
    {
        public Guid Id { get; set; }

        public Guid QuizAttemptId { get; set; }
        public QuizAttempt QuizAttempt { get; set; } = null!;

        public Guid QuestionId { get; set; }
        public Question Question { get; set; } = null!;

        public Guid? SelectedOptionId { get; set; }
        public QuestionOption? SelectedOption { get; set; }

        public bool IsCorrect { get; set; } = false;
        public int EarnedPoints { get; set; } = 0;
    }
}