namespace Workouts
{
    public class WorkoutEntityToDetailedDtoMapper : IMapper<Workout, WorkoutDetailedDto>
    {
        public WorkoutDetailedDto Map(Workout src)
        {
            return new WorkoutDetailedDto
            {
                Name = src.Name,
                Date = src.Date,
                Notes = src.Notes,
                Exercises = src.Exercises.Select(we => we.Exercise.Name).ToList(),
                MuscleGroups = src.MuscleGroups.Select(mg => mg.Name).ToList()
            };
        }
    }
}
