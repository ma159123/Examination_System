using System.ComponentModel.DataAnnotations;

namespace Domain.Entites
{
    public class OtpRecord
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; }
        public string HashedOtp { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsUsed { get; set; }
    }
}
