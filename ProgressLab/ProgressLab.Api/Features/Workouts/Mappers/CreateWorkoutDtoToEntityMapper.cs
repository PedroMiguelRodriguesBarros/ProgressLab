namespace Workouts
{
    public class CreateWorkoutDtoToEntityMapper : IMapper<CreateWorkoutDto, Workout>
    {
        public Workout Map(CreateWorkoutDto src)
        {
            return new Workout(
                src.Name.Trim(),
                src.Date ?? DateTimeOffset.UtcNow,
                src.Notes?.Trim()
            );
        }
    }
}