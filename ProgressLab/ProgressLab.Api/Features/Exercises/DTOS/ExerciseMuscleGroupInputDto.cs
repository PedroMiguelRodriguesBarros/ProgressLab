using ExercisesMuscleGroups;

namespace Exercises
{
    public class ExerciseMuscleGroupInputDto
    {
        public Guid MuscleGroupId { get; set; }

        public MuscleGroupRole Role { get; set; }
    }
}