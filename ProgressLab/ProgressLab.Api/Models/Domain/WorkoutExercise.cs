using Sets;
using Exercises;

namespace Workouts
{
    public class WorkoutExercise
    {
        public Guid WorkoutExerciseId { get; set; }
        public Guid WorkoutId { get; set; }
        public Workout Workout { get; set; } = null!;
        public Guid ExerciseId { get; set; }
        public Exercise Exercise { get; set; } = null!;
        public int Order { get; set; }
        public List<Set> Sets { get; set; } = new();


        public WorkoutExercise() { }

        public WorkoutExercise(Guid workoutId, Guid exerciseId, int order)
        {
            WorkoutExerciseId = Guid.NewGuid();
            WorkoutId = workoutId;
            ExerciseId = exerciseId;
            Order = order;
        }
    }
}