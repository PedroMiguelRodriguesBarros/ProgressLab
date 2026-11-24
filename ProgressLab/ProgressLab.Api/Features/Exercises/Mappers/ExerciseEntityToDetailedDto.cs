namespace Exercises
{
    public class ExerciseEntityToDetailedDtoMapper : IMapper<Exercise, ExerciseDetailedDto>
    {
        public ExerciseDetailedDto Map(Exercise src)
        {
            return new ExerciseDetailedDto
            {   
                ExerciseId = src.ExerciseId,
                Name = src.Name.Trim(),
                Description = src.Description?.Trim(),
                MuscleGroups = src.ExerciseMuscleGroups.Select(emg => new ExerciseMuscleGroupDto
                {
                    MuscleGroupId = emg.MuscleGroupId,
                    MuscleGroupName = emg.MuscleGroup.Name,
                    Role = emg.Role
                }).ToList()
            };
        }
    }
}
