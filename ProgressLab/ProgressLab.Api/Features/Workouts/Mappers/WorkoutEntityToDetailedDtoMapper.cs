using Sets;

namespace Workouts
{
    public class WorkoutEntityToDetailedDtoMapper : IMapper<Workout, WorkoutDetailedDto>
    {
        public WorkoutDetailedDto Map(Workout src)
        {
            return new WorkoutDetailedDto
            {
                WorkoutId = src.WorkoutId,
                Name = src.Name,
                Date = src.Date,
                Notes = src.Notes,
                WorkoutExercises = src.WorkoutExercises.Select(we => new WorkoutExerciseDetailedDto
                {
                    WorkoutExerciseId = we.WorkoutExerciseId,
                    ExerciseId = we.ExerciseId,
                    ExerciseName = we.Exercise.Name,
                    Sets = we.Sets.Select(s => new SetDetailedDto
                    {
                        SetId = s.SetId,
                        Repetitions = s.Repetitions,
                        Weight = s.Weight
                    }).ToList()
                }).ToList()
            };
        }
    }
}
