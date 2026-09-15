using Domain.Enums;
using System.Text.Json.Serialization;

namespace Application.DTOs
{
    public class GetQuizResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }

        public int AttemptCount { get; set; } = 0;

        public int? MaxAttempts { get; set; } = 0;

        public bool CanAttempt { get; set; } = true;
        public int? PassScore { get; set; }

        public int? LastScore { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public QuizAttemptStatus Status { get; set; } = QuizAttemptStatus.NotStarted;
    }
}