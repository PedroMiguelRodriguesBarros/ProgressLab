using Sets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations
{
    public class SetConfiguration : IEntityTypeConfiguration<Set>
    {
        public void Configure(EntityTypeBuilder<Set> builder)
        {
            builder.HasKey(s => s.SetId);

            builder.Property(s => s.SetNumber)
                .IsRequired();

            builder.Property(s => s.Repetitions)
                .IsRequired();

            builder.Property(s => s.Weight)
                .IsRequired()
                .HasPrecision(6, 2);
            
            builder.HasIndex(s => new
            {
                s.WorkoutExerciseId,
                s.SetNumber
            }).IsUnique();
        }
    }
}