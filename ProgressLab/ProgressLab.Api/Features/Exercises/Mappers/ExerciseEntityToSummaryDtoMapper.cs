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
            };
        }
    }
}