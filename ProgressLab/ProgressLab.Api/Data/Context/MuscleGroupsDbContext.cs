using MuscleGroups;
using Microsoft.EntityFrameworkCore;

namespace Data.Context
{
    public class MuscleGroupsDbContext : DbContext
    {
        public MuscleGroupsDbContext(DbContextOptions<MuscleGroupsDbContext> options) : base(options)
        {
        }

        public DbSet<MuscleGroup> MuscleGroups => Set<MuscleGroup>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<MuscleGroup>().HasKey(it => it.MuscleGroupId);
            
            modelBuilder.Entity<MuscleGroup>()
                .Property(it => it.Name)
                .IsRequired()
                .HasMaxLength(255);
            
            modelBuilder.Entity<MuscleGroup>()
                .Property(it => it.Description)
                .IsRequired()
                .HasMaxLength(2000);
        }
    }
}
