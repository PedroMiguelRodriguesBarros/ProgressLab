using Exercises;
using MuscleGroups;

namespace Workouts {
    public class Workout {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public List<Exercise> Exercises { get; set; }
        public List<MuscleGroup> MuscleGroups { get; set; }

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