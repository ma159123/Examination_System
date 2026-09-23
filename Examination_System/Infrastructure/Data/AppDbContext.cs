using Domain.Entites;
using Domain.Entities;
using Infrastructure.DataSeeding;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<string>, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<OtpRecord> OtpRecords { get; set; }
        public DbSet<ResetTokenRecord> ResetTokenRecords { get; set; }
        public DbSet<RefreshTokenRecord> RefreshTokenRecords { get; set; }
        public DbSet<Diploma> Diplomas { get; set; }
        public DbSet<StudentDiploma> StudentDiplomas { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionOption> QuestionOptions { get; set; }
        public DbSet<QuizAttempt> QuizAttempts { get; set; }
        public DbSet<QuestionAnswer> QuestionAnswers { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            // Seed Data
            DbInitializer.Seed(builder);
        }
    }

}

