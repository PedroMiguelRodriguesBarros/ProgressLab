using MuscleGroups;
using Workouts;

namespace Exercises {
    public class Exercise {
        public Guid ExerciseId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public MuscleGroup MuscleGroup { get; set; }
        public List<WorkoutExercise> WorkoutExercises { get; set; }

        public Exercise() {
        }

        public Exercise(string name, string description,  MuscleGroup muscleGroup) {
            ExerciseId = Guid.NewGuid();
            Name = name;
            Description = description;
            MuscleGroup = muscleGroup;
        }
    }
}