using Domain.Entites;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class DiplomaConfiguration : IEntityTypeConfiguration<Diploma>
    {
        public void Configure(EntityTypeBuilder<Diploma> builder)
        {
            builder.ToTable("Diplomas");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(d => d.Description)
                .HasMaxLength(1000);

            builder.Property(d => d.Status)
                .IsRequired()
                .HasConversion<int>()
                .HasDefaultValue(ContentStatus.Draft);

            builder.Property(d => d.CreatedAt)
                .IsRequired();

            // Global Query Filter for Soft Delete
            builder.HasQueryFilter(d => d.DeletedAt == null);

            // Navigation Relationships
            builder.HasMany(d => d.Quizzes)
                .WithOne(q => q.Diploma)
                .HasForeignKey(q => q.DiplomaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
