using Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entites
{
    public class AppUser : IdentityUser<string>
    {
        public string FullName { get; set; }
        public UserStatus Status { get; set; }
        public UserRoles Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
