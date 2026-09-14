namespace Domain.Entites
{
    public class StudentDiploma
    {
        public Guid StudentId { get; set; }

        public Guid DiplomaId { get; set; }
        public Diploma Diploma { get; set; } = null!;

        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    }
}
