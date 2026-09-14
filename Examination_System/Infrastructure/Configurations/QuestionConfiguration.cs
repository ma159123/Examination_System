using Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.Configurations
{

    public class QuestionConfiguration : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.ToTable("Questions");

            builder.HasKey(q => q.Id);

            builder.Property(q => q.Text)
                .IsRequired();

            builder.Property(q => q.OrderIndex)
                .IsRequired();

            // Global Query Filter for Soft Delete
            builder.HasQueryFilter(q => q.DeletedAt == null);

            // Index on Foreign Key for query performance
            builder.HasIndex(q => q.QuizId);

            // Navigation Relationships
            builder.HasMany(q => q.Options)
                .WithOne(o => o.Question)
                .HasForeignKey(o => o.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
