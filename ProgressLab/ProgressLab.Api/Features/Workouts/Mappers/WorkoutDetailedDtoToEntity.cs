using MuscleGroups;
using Exercises;

namespace Workouts
{
    public class WorkoutDetailedDtoToEntityMapper : IMapper<WorkoutDetailedDto, Workout>
    {
        public Workout Map(WorkoutDetailedDto src)
        {
            return new Workout
            {
                Name = src.Name,
                Notes = src.Notes,
                Exercises = src.Exercises.Select(name => new WorkoutExercise { Exercise = new Exercise { Name = name } }).ToList(),
                MuscleGroups = src.MuscleGroups.Select(name => new MuscleGroup { Name = name }).ToList()
            };
        }
    }
}
