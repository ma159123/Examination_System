using Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{

    public class StudentDiplomaConfiguration : IEntityTypeConfiguration<StudentDiploma>
    {
        public void Configure(EntityTypeBuilder<StudentDiploma> builder)
        {
            builder.ToTable("StudentDiplomas");

            // Composite Key
            builder.HasKey(sd => new { sd.StudentId, sd.DiplomaId });

            builder.HasIndex(sd => sd.StudentId);

            builder.HasOne(sd => sd.Diploma)
                .WithMany()
                .HasForeignKey(sd => sd.DiplomaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
