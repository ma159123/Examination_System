using Domain.Entites;
using Infrastructure.DataSeeding;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<OtpRecord> OtpRecords { get; set; }
        public DbSet<ResetTokenRecord> ResetTokenRecords { get; set; }
        public DbSet<RefreshTokenRecord> RefreshTokenRecords { get; set; }
        public DbSet<Diploma> Diplomas { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionOption> QuestionOptions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            // Seed Data
            DbInitializer.Seed(builder);
        }
    }

}

