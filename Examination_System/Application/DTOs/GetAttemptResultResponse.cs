namespace Application.DTOs
{
    public class GetAttemptResultResponse
    {
        public int Score { get; set; }
        public bool Passed { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectCount { get; set; }

        public List<QuestionResultResponse> QuestionResultResponses { get; set; } = new List<QuestionResultResponse>();
    }
}
