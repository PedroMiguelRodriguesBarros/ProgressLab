using Workouts;

namespace Sets {
    public class Set {
        public Guid SetId { get; set; }
        public int SetNumber { get; set; }
        public int Repetitions { get; set; }
        public decimal Weight { get; set; }
        public Guid WorkoutExerciseId { get; set; }
        public WorkoutExercise WorkoutExercise { get; set; } = null!;

        public Set() {
        }

        public Set(int setNumber, int repetitions, decimal weight, WorkoutExercise workoutExercise) {
            SetId = Guid.NewGuid();
            SetNumber = setNumber;
            Repetitions = repetitions;
            Weight = weight;
            WorkoutExercise = workoutExercise;
        }
    }
}