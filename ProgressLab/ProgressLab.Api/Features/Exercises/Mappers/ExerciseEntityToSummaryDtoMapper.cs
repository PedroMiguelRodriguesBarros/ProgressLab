namespace Exercises
{
    public class ExerciseEntityToSummaryDtoMapper : IMapper<Exercise, ExerciseSummaryDto>
    {
        public ExerciseSummaryDto Map(Exercise src)
        {
            return new ExerciseSummaryDto
            {
                ExerciseId = src.ExerciseId,
                Name = src.Name,
                PrimaryMuscleGroup = src.ExerciseMuscleGroups
                .FirstOrDefault(emg => emg.Role == MuscleGroupRole.Primary)?.MuscleGroup.Name ?? string.Empty,
            };
        }
    }
}