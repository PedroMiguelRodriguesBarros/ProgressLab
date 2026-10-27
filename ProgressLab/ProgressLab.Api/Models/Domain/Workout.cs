using Exercises;
using MuscleGroups;

namespace Workouts {
    public class Workout {
        private Guid Id { get; set; }
        private string Name { get; set; }
        private DateTimeOffset CreatedAt { get; set; }
        private List<Exercise> Exercises { get; set; }
        private List<MuscleGroup> MuscleGroups { get; set; }

        public Workout() {
        }

        public Workout(string name, List<Exercise> exercises, List<MuscleGroup> muscleGroups) {
            Id = Guid.NewGuid();
            Name = name;
            CreatedAt = DateTimeOffset.UtcNow;
            Exercises = exercises;
            MuscleGroups = muscleGroups;
        }
    }
}