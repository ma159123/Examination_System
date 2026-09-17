namespace Application.DTOs
{
    public class StartQuizResponse
    {
        public Guid AttemptId { get; set; }
        public Guid QuizId { get; set; }
        public string QuizTitle { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime Deadline { get; set; }
        public int TotalQuestions { get; set; }
        public List<QuestionDto> Questions { get; set; } = new();
    }




}
