using Exercises;
using MuscleGroups;

namespace Workouts {
    public class Workout {
        public Guid WorkoutId { get; set; }
        public string Name { get; set; }
        public DateTimeOffset Date { get; set; }
        public string? Notes { get; set; }
        public List<WorkoutExercise> Exercises { get; set; }
        public List<MuscleGroup> MuscleGroups { get; set; }

        public Workout() {
        }

        public Workout(string name, string? notes, List<WorkoutExercise> exercises, List<MuscleGroup> muscleGroups) {
            WorkoutId = Guid.NewGuid();
            Name = name;
            Date = DateTimeOffset.UtcNow;
            Notes = notes;
            Exercises = exercises;
            MuscleGroups = muscleGroups;
        }
    }
}