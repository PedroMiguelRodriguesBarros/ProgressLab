using Workouts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations
{
    public class WorkoutConfiguration : IEntityTypeConfiguration<Workout>
    {
        public void Configure(EntityTypeBuilder<Workout> builder)
        {
            builder.HasKey(w => w.WorkoutId);

            builder.Property(w => w.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(w => w.Date)
                .IsRequired();

            builder.Property(w => w.Notes)
                .HasMaxLength(1000);
        }
    }
}