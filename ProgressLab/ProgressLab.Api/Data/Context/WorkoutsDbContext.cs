using Workouts;
using MuscleGroups;
using Exercises;
using Microsoft.EntityFrameworkCore;

namespace Data.Context
{
    public class WorkoutsDbContext : DbContext
    {
        public WorkoutsDbContext(DbContextOptions<WorkoutsDbContext> options) : base(options)
        {
        }

        public DbSet<Workout> Workouts => Set<Workout>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Workout>().HasKey(it => it.WorkoutId);
            
            modelBuilder.Entity<Workout>()
                .Property(it => it.Name)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Entity<Workout>()
                .Property(it => it.Date)
                .IsRequired()
                .HasColumnType("datetimeoffset");

            modelBuilder.Entity<Workout>()
                .HasMany(it => it.Exercises)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "WorkoutExercise",
                    j => j.HasOne<Exercise>().WithMany().HasForeignKey("ExerciseId"),
                    j => j.HasOne<Workout>().WithMany().HasForeignKey("WorkoutId")
                );
            
            modelBuilder.Entity<Workout>()
                .HasMany(it => it.MuscleGroups)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "WorkoutMuscleGroup",
                    j => j.HasOne<MuscleGroup>().WithMany().HasForeignKey("MuscleGroupId"),
                    j => j.HasOne<Workout>().WithMany().HasForeignKey("WorkoutId")
                );
        }
    }
}
