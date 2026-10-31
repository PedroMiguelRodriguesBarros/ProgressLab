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

        public async Task<Workout?> CreateAsync(string name, string? notes, List<Guid> exerciseIds, CancellationToken ct = default)
        {
            var exercises = exerciseIds.Select(id => new WorkoutExercise { Exercise = new Exercise { ExerciseId = id } }).ToList();
            var muscleGroups = new List<MuscleGroup>();

            muscleGroups.AddRange(exercises.Select(e => e.Exercise.MuscleGroup).Distinct().ToList());

            var workout = new Workout(name, notes, exercises, muscleGroups);
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
            var updatedMuscleGroups = updatedExercises.Select(e => e.Exercise.MuscleGroup).Distinct().ToList();

            workout.Name = dto.Name;
            workout.Date = dto.Date;
            workout.Notes = dto.Notes;
            workout.Exercises = updatedExercises;
            workout.MuscleGroups = updatedMuscleGroups;

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
