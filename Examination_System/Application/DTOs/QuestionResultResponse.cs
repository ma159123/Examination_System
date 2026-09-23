namespace Application.DTOs
{
    public class QuestionResultResponse
    {
        public Guid QuestionId { get; set; }
        public string StudentAnswer { get; set; }
        public string CorrectAnswer { get; set; }
        public bool IsCorrect { get; set; }
    }
}
