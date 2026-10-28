namespace Exercises
{
    public class ExerciseEntityToDetailedDtoMapper : IMapper<Exercise, ExerciseDetailedDto>
    {
        public ExerciseDetailedDto Map(Exercise src)
        {
            return new ExerciseDetailedDto
            {
                Name = src.Name,
                Description = src.Description,
                Duration = src.Duration,
                Sets = src.Sets.Select(s => s.ToString()).ToList(),
                MuscleGroup = src.MuscleGroup.ToString()
            };
        }
    }
}
