namespace Domain.Entites
{
    public class StudentDiploma
    {
        public Guid StudentId { get; set; }

        public Guid DiplomaId { get; set; }
        public Diploma Diploma { get; set; } = null!;

        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

        // Persisted Progress Field
        public int ProgressPercentage { get; set; } = 0;
        public bool IsCompleted { get; set; } = false;
        public DateTime? CompletedAt { get; set; }

        public void UpdateProgress(int completedQuizzesCount, int totalQuizzesCount)
        {
            if (totalQuizzesCount <= 0)
            {
                ProgressPercentage = 0;
                return;
            }

            ProgressPercentage = (int)Math.Round((double)completedQuizzesCount / totalQuizzesCount * 100);

            if (ProgressPercentage >= 100 && !IsCompleted)
            {
                ProgressPercentage = 100;
                IsCompleted = true;
                CompletedAt = DateTime.UtcNow;
            }
        }
    }
}
