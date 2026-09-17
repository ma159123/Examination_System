namespace Application.DTOs
{
    public class QuestionDto
    {
        public Guid QuestionId { get; set; }
        public int Order { get; set; }
        public string Text { get; set; } = string.Empty;
        public List<OptionDto> Options { get; set; } = new();
    }
}
