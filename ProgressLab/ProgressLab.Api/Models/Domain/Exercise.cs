using ExercisesMuscleGroups;
using Workouts;

namespace Exercises {
    public class Exercise {
        public Guid ExerciseId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<ExerciseMuscleGroup> ExerciseMuscleGroups { get; set; } = new();
        public List<WorkoutExercise> WorkoutExercises { get; set; } = new();

        public Exercise() {
        }

        public Exercise(string name, string description) {
            ExerciseId = Guid.NewGuid();
            Name = name;
            Description = description;
        }

        public void AddMuscleGroups(List<ExerciseMuscleGroup> muscleGroups) {
            ExerciseMuscleGroups.AddRange(muscleGroups);
        }
    }
}