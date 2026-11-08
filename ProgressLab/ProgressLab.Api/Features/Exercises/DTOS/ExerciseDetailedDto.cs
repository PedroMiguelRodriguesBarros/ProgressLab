namespace Exercises
{
    public class ExerciseDetailedDto
    {
        public Guid ExerciseId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public List<ExerciseMuscleGroupDto> MuscleGroups { get; set; } = new();
    }
}