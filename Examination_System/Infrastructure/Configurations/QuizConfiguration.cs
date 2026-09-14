using Domain.Entites;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{

    public class QuizConfiguration : IEntityTypeConfiguration<Quiz>
    {
        public void Configure(EntityTypeBuilder<Quiz> builder)
        {
            builder.ToTable("Quizzes");

            builder.HasKey(q => q.Id);

            builder.Property(q => q.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(q => q.DurationMinutes)
                .IsRequired();

            builder.Property(q => q.PassScore)
                .IsRequired()
                .HasDefaultValue(60);

            builder.Property(q => q.Status)
                .IsRequired()
                .HasConversion<int>()
                .HasDefaultValue(ContentStatus.Draft);

            // Index on Foreign Key for performance optimization
            builder.HasIndex(q => q.DiplomaId);

            // Navigation Relationships
            builder.HasMany(q => q.Questions)
                .WithOne(q => q.Quiz)
                .HasForeignKey(q => q.QuizId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
