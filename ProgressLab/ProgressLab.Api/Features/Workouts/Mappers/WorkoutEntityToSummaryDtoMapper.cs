namespace Workouts
{
    public class WorkoutEntityToSummaryDtoMapper : IMapper<Workout, WorkoutSummaryDto>
    {
        public WorkoutSummaryDto Map(Workout src)
        {
            return new WorkoutSummaryDto
            {
                WorkoutId = src.WorkoutId,
                Name = src.Name,
                Date = src.Date,
            };
        }
    }
}