namespace Workouts
{
    public class WorkoutDetailedDtoToEntityMapper : IMapper<WorkoutDetailedDto, Workout>
    {
        public Workout Map(WorkoutDetailedDto src)
        {
            return new Workout
            {
                Name = src.Name,
                Date = src.Date,
                ExerciseNames = src.ExerciseNames,
                MuscleGroupNames = src.MuscleGroupNames
            };
        }
    }
}
