using Sets;

namespace Workouts
{
    public class WorkoutExercise
    {
        public Guid Id { get; set; }
        public Guid WorkoutId { get; set; }
        public Guid ExerciseId { get; set; }
        public int Order { get; set; }
        public List<Set> Sets { get; set; }


        public WorkoutExercise() { }

        public WorkoutExercise(Guid workoutId, Guid exerciseId, int order)
        {
            Id = Guid.NewGuid();
            WorkoutId = workoutId;
            ExerciseId = exerciseId;
            Order = order;
        }
    }
}