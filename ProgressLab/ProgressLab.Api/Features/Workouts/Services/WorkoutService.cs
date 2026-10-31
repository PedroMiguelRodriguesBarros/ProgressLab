using MuscleGroups;
using Exercises;

namespace Workouts
{
    public class WorkoutService
    {
        private readonly IWorkoutRepository _repo;

        public WorkoutService(IWorkoutRepository repo)
        {
            _repo = repo;
        }

        public async Task<Workout> GetByIdAsync(Guid workoutId, CancellationToken ct = default)
        {
            return await _repo.GetByIdAsync(workoutId, ct);
        }

        public async Task<List<Workout>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
        {
            return await _repo.GetAllAsync(page, pageSize, ct);
        }

        public async Task<Workout?> CreateAsync(CreateWorkoutDto dto, CancellationToken ct = default)
        {
            var workout = new Workout(dto.Name, dto.Notes);
            return await _repo.AddAsync(workout, ct);
        }

        public async Task<Workout?> UpdateWorkoutAsync(Guid workoutId, UpdateWorkoutDto dto, CancellationToken ct = default)
        {
            var workout = await _repo.GetByIdAsync(workoutId, ct);
            if (workout == null)
            {
                return null;
            }

            var updatedExercises = dto.Exercises.Select(id => new WorkoutExercise { Exercise = new Exercise { ExerciseId = id } }).ToList();

            workout.Name = dto.Name;
            workout.Date = dto.Date;
            workout.Notes = dto.Notes;

            return await _repo.UpdateAsync(workout, ct);
        }

        public async Task<bool> DeleteAsync(Guid workoutId, CancellationToken ct = default)
        {
            var workout = await _repo.GetByIdAsync(workoutId, ct);
            if (workout == null)
            {
                return false;
            }

            await _repo.DeleteAsync(workout, ct);
            return true;
        }
    }
}
