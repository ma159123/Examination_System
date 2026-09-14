using Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{

    public class QuestionOptionConfiguration : IEntityTypeConfiguration<QuestionOption>
    {
        public void Configure(EntityTypeBuilder<QuestionOption> builder)
        {
            builder.ToTable("QuestionOptions");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Text)
                .IsRequired();

            builder.Property(o => o.IsCorrect)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(o => o.QuestionId);
        }
    }
}
