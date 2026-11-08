using ExercisesMuscleGroups;

namespace Exercises
{
    public class ExerciseMuscleGroupDto
    {
        public Guid MuscleGroupId { get; set; }

        public string MuscleGroupName { get; set; } = string.Empty;

        public MuscleGroupRole Role { get; set; }
    }
}