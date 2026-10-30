using Exercises;
using MuscleGroups;

namespace Workouts {
    public class Workout {
        public Guid WorkoutId { get; set; }
        public string Name { get; set; }
        public DateTimeOffset Date { get; set; }
        public List<Exercise> Exercises { get; set; }
        public List<MuscleGroup> MuscleGroups { get; set; }

        public Workout() {
        }

        public Workout(string name, List<Exercise> exercises, List<MuscleGroup> muscleGroups) {
            WorkoutId = Guid.NewGuid();
            Name = name;
            Date = DateTimeOffset.UtcNow;
            Exercises = exercises;
            MuscleGroups = muscleGroups;
        }
    }
}