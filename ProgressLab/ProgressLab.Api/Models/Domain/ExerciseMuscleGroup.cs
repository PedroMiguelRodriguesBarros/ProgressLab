using MuscleGroups;
using Exercises;

namespace ExercisesMuscleGroups {
    public class ExerciseMuscleGroup {
        public Guid ExerciseId { get; set; }
        public Exercise Exercise { get; set; } = null!;
        public Guid MuscleGroupId { get; set; }
        public MuscleGroup MuscleGroup { get; set; } = null!;
        public MuscleGroupRole Role { get; set; }

        public ExerciseMuscleGroup() {
        }

        public ExerciseMuscleGroup(Exercise exercise, MuscleGroup muscleGroup, MuscleGroupRole role) {
            ExerciseId = exercise.ExerciseId;
            Exercise = exercise;
            MuscleGroupId = muscleGroup.MuscleGroupId;
            MuscleGroup = muscleGroup;
            Role = role;
        }
    }
}