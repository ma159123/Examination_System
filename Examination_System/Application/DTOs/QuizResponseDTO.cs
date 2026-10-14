using Domain.Enums;

namespace Application.DTOs
{
    public class QuizResponseDTO
    {
        public Guid QuizId { get; internal set; }
        public Guid DiplomaId { get; set; }
        public string Title { get; set; }
        public int DurationMinutes { get; set; }
        public int PassScore { get; set; }
        public int? MaxAttempts { get; set; }
        public ContentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

    }
}
