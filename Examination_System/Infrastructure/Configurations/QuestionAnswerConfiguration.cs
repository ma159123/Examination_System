using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class QuestionAnswerConfiguration : IEntityTypeConfiguration<QuestionAnswer>
    {
        public void Configure(EntityTypeBuilder<QuestionAnswer> builder)
        {
            builder.HasKey(qa => qa.Id);


            builder.HasOne(qa => qa.QuizAttempt)
                   .WithMany(attempt => attempt.Answers)
                   .HasForeignKey(qa => qa.QuizAttemptId)
                   .OnDelete(DeleteBehavior.Cascade);


            builder.HasOne(qa => qa.Question)
                   .WithMany()
                   .HasForeignKey(qa => qa.QuestionId)
                   .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(qa => qa.SelectedOption)
                   .WithMany()
                   .HasForeignKey(qa => qa.SelectedOptionId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(qa => new { qa.QuizAttemptId, qa.QuestionId }).IsUnique();
        }
    }
}
