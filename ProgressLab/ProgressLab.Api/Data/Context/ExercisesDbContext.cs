using Exercises;
using Microsoft.EntityFrameworkCore;

namespace Data.Context
{
    public class ExercisesDbContext : DbContext
    {
        public ExercisesDbContext(DbContextOptions<ExercisesDbContext> options) : base(options)
        {
        }

        public DbSet<Exercise> Exercises => Set<Exercise>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Exercise>().HasKey(it => it.ExerciseId);
            
            modelBuilder.Entity<Exercise>()
                .Property(it => it.Name)
                .IsRequired()
                .HasMaxLength(255);
            
            modelBuilder.Entity<Exercise>()
                .Property(it => it.Description)
                .IsRequired()
                .HasMaxLength(2000);
            
            modelBuilder.Entity<Exercise>()
                .HasOne(it => it.MuscleGroup)
                .WithMany()
                .HasForeignKey("MuscleGroupId");
        }
    }
}
