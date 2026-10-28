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
                ExerciseNames = src.Exercises.Select(e => e.Name).ToList(),
                MuscleGroupNames = src.MuscleGroups.Select(mg => mg.Name).ToList()
            };
        }
    }
}
