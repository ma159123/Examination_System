using Domain.Enums;

namespace Application.DTOs
{
    public class GetDiplomaResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int QuizCount { get; set; } = 0;
        public bool IsEnrolled { get; set; } = false;
        public int StudentProgress { get; set; } = 0;
        public ContentStatus Status { get; set; } = ContentStatus.Draft;

    }
}
