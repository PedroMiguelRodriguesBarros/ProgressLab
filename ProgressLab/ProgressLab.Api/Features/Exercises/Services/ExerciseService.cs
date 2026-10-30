using MuscleGroups;

namespace Exercises
{
    public class ExerciseService
    {
        private readonly IExerciseRepository _repo;
        private readonly IMuscleGroupRepository _muscleGroupRepo;

        public ExerciseService(IExerciseRepository repo, IMuscleGroupRepository muscleGroupRepo)
        {
            _repo = repo;
            _muscleGroupRepo = muscleGroupRepo;
        }

        public async Task<Exercise> GetByIdAsync(Guid exerciseId, CancellationToken ct = default)
        {
            return await _repo.GetByIdAsync(exerciseId, ct);
        }

        public async Task<List<Exercise>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
        {
            return await _repo.GetAllAsync(page, pageSize, ct);
        }

        public async Task<Exercise?> CreateAsync(string name, string description, Guid muscleGroupId, CancellationToken ct = default)
        {
            var muscleGroup = await _muscleGroupRepo.GetByIdAsync(muscleGroupId, ct);
            if (muscleGroup == null)
            {
                return null;
            }
            var exercise = new Exercise(name, description, muscleGroup);
            return await _repo.AddAsync(exercise, ct);
        }

        public async Task<Exercise?> UpdateExerciseAsync(Guid exerciseId, CancellationToken ct = default)
        {
            var exercise = await _repo.GetByIdAsync(exerciseId, ct);
            if (exercise == null)
            {
                return null;
            }

            return await _repo.UpdateAsync(exercise, ct);
        }

        public async Task DeleteAsync(Guid exerciseId, CancellationToken ct = default)
        {
            var exercise = await _repo.GetByIdAsync(exerciseId, ct);
            if (exercise == null)
            {
                return;
            }
            await _repo.DeleteAsync(exercise, ct);
        }
    }
}
