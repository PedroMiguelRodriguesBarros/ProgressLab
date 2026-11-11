using Exercises;

namespace Workouts
{
    public class WorkoutService
    {
        private readonly IWorkoutRepository _repo;
        private readonly IExerciseRepository _exerciseRepo;
        private readonly IMapper<CreateWorkoutDto, Workout> _createMapper;
        private readonly IMapper<Workout, WorkoutSummaryDto> _summaryMapper;
        private readonly IMapper<Workout, WorkoutDetailedDto> _detailedMapper;

        public WorkoutService(IWorkoutRepository repo, IExerciseRepository exerciseRepo, IMapper<CreateWorkoutDto, Workout> createMapper, IMapper<Workout, WorkoutSummaryDto> summaryMapper, IMapper<Workout, WorkoutDetailedDto> detailedMapper)
        {
            _repo = repo;
            _exerciseRepo = exerciseRepo;
            _createMapper = createMapper;
            _summaryMapper = summaryMapper;
            _detailedMapper = detailedMapper;
        }

        public async Task<WorkoutDetailedDto?> GetByIdAsync(Guid workoutId, CancellationToken ct = default)
        {
            var workout = await _repo.GetByIdAsync(workoutId, ct);
            if (workout == null)
            {
                return null;
            }

            return _detailedMapper.Map(workout);
        }

        public async Task<IReadOnlyList<WorkoutSummaryDto>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
        {
            var workouts = await _repo.GetAllAsync(page, pageSize, ct);

            return workouts.Select(w => _summaryMapper.Map(w)).ToList();
        }

        public async Task<WorkoutDetailedDto> CreateAsync(CreateWorkoutDto dto, CancellationToken ct = default)
        {
            var workout = _createMapper.Map(dto);
            var createdWorkout = await _repo.AddAsync(workout, ct);

            return _detailedMapper.Map(createdWorkout);
        }

        public async Task<WorkoutDetailedDto?> UpdateAsync(Guid workoutId, UpdateWorkoutDto dto, CancellationToken ct = default)
        {
            var workout = await _repo.GetByIdForUpdateAsync(workoutId, ct);
            if (workout == null)
            {
                return null;
            }

            workout.Name = dto.Name.Trim();

            if (dto.Date.HasValue)
            {
                workout.Date = dto.Date.Value;
            }   

            workout.Notes = dto.Notes?.Trim();

            await _repo.UpdateAsync(workout, ct);

            var updatedWorkout = await _repo.GetByIdAsync(workoutId, ct);

            return updatedWorkout == null
                ? null
                : _detailedMapper.Map(updatedWorkout);
        }

        public async Task<bool> DeleteAsync(Guid workoutId, CancellationToken ct = default)
        {
            var workout = await _repo.GetByIdForUpdateAsync(workoutId, ct);
            if (workout == null)
            {
                return false;
            }

           return await _repo.DeleteAsync(workout, ct);
        }

        public async Task AddExerciseAsync(Guid workoutId, Guid exerciseId, CancellationToken ct = default)
        {
            var workout = await _repo.GetByIdForUpdateAsync(workoutId, ct);
            if (workout == null)
            {
                throw new ArgumentException($"Workout with ID {workoutId} does not exist.");
            }

            var exerciseExists = await _exerciseRepo.ExistsAsync(exerciseId, ct);

            if (!exerciseExists)
            {
                throw new ArgumentException($"Exercise with ID {exerciseId} does not exist.");
            }

            if (workout.WorkoutExercises.Any(we => we.ExerciseId == exerciseId))
            {
                throw new InvalidOperationException($"Exercise is already part of the workout.");
            }

            var order = workout.WorkoutExercises
                .Select(we => we.Order)
                .DefaultIfEmpty(0)
                .Max() + 1;

            var workoutExercise = new WorkoutExercise(
                workout.WorkoutId,
                exerciseId,
                order
            );

            workout.WorkoutExercises.Add(workoutExercise);

            await _repo.UpdateAsync(workout, ct);
        }

        public async Task RemoveExerciseAsync(Guid workoutId, Guid exerciseId, CancellationToken ct = default)
        {
            var workout = await _repo.GetByIdForUpdateAsync(workoutId, ct);
            if (workout == null)
            {
                throw new ArgumentException($"Workout with ID {workoutId} does not exist.");
            }

            var workoutExercise = workout.WorkoutExercises.FirstOrDefault(we => we.ExerciseId == exerciseId);

            if (workoutExercise == null)
            {
                throw new InvalidOperationException($"Exercise is not part of the workout.");
            }

            workout.WorkoutExercises.Remove(workoutExercise);

            var remainingExercises = workout.WorkoutExercises
                .OrderBy(we => we.Order)
                .ToList();

            for (int i = 0; i < remainingExercises.Count; i++)
            {
                remainingExercises[i].Order = i + 1;
            }

            await _repo.UpdateAsync(workout, ct);
        }

        public async Task UpdateExerciseOrderAsync(Guid workoutId, Guid exerciseId, int newOrder, CancellationToken ct = default)
        {
            var workout = await _repo.GetByIdForUpdateAsync(workoutId, ct);

            if (workout == null)
            {
                throw new ArgumentException($"Workout with ID {workoutId} does not exist.");
            }

            var workoutExercises = workout.WorkoutExercises
                .OrderBy(we => we.Order)
                .ToList();

            var workoutExercise = workoutExercises
                .FirstOrDefault(we => we.ExerciseId == exerciseId);

            if (workoutExercise == null)
            {
                throw new InvalidOperationException("Exercise is not part of the workout.");
            }

            if (newOrder < 1 || newOrder > workoutExercises.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(newOrder), "The new order must be within the workout's exercise positions.");
            }

            workoutExercises.Remove(workoutExercise);

            workoutExercises.Insert(newOrder - 1, workoutExercise);

            for (int i = 0; i < workoutExercises.Count; i++)
            {
                workoutExercises[i].Order = i + 1;
            }

            await _repo.UpdateAsync(workout, ct);
        }
    }
}
