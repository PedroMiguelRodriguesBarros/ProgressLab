namespace Exercises
{
    public class ExerciseSummaryDto
    {
        public Guid ExerciseId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string PrimaryMuscleGroup { get; set; } = string.Empty;
    }
}