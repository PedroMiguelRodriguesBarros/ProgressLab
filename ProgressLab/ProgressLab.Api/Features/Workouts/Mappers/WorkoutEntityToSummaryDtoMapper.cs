namespace Workouts
{
    public class WorkoutEntityToSummaryDtoMapper : IMapper<Workout, WorkoutSummaryDto>
    {
        public WorkoutSummaryDto Map(Workout src)
        {
            return new WorkoutSummaryDto
            {
                Name = src.Name,
                Date = src.Date,
            };
        }
    }
}