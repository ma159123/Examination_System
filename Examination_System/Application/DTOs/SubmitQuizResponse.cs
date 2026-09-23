namespace Application.DTOs
{
    public class SubmitQuizResponse
    {
        public Guid AttemptId { get; set; }
        public int Score { get; set; }
        public bool Passed { get; set; }
    }
}

