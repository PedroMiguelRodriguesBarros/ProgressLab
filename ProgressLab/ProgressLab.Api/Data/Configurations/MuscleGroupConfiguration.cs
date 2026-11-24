using MuscleGroups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations
{
    public class MuscleGroupConfiguration : IEntityTypeConfiguration<MuscleGroup>
    {
        public void Configure(EntityTypeBuilder<MuscleGroup> builder)
        {
            builder.HasKey(mg => mg.MuscleGroupId);

            builder.Property(mg => mg.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(mg => mg.Description)
                .HasMaxLength(2000);
        }
    }
}