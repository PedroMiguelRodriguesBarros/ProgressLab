using Exercises;
using ExercisesMuscleGroups;
using MuscleGroups;
using Sets;
using Workouts;
using Microsoft.EntityFrameworkCore;

namespace Data.Context
{
    public class ProgressLabDbContext : DbContext
    {
        public ProgressLabDbContext(
            DbContextOptions<ProgressLabDbContext> options) : base(options)
        {
        }

        public DbSet<Exercise> Exercises => Set<Exercise>();
        public DbSet<MuscleGroup> MuscleGroups => Set<MuscleGroup>();
        public DbSet<ExerciseMuscleGroup> ExerciseMuscleGroups => Set<ExerciseMuscleGroup>();
        public DbSet<Workout> Workouts => Set<Workout>();
        public DbSet<WorkoutExercise> WorkoutExercises => Set<WorkoutExercise>();
        public DbSet<Set> Sets => Set<Set>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(ProgressLabDbContext).Assembly
            );
        }
    }
}