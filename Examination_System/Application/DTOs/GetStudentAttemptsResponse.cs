namespace Application.DTOs
{
    public class GetStudentAttemptResponse
    {
        public Guid AttemptId { get; set; }
        public Guid QuizId { get; set; }
        public string QuizTitle { get; set; } = string.Empty;
        public Guid DiplomaId { get; set; }
        public string DiplomaTitle { get; set; } = string.Empty;
        public int Score { get; set; }
        public bool IsPassed { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
    }
}
