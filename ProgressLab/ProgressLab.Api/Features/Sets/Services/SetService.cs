using Exercises;

namespace Sets
{
    public class SetService
    {
        private readonly ISetRepository _repo;
        private readonly IExerciseRepository _exerciseRepo;

        public SetService(ISetRepository repo, IExerciseRepository exerciseRepo)
        {
            _repo = repo;
            _exerciseRepo = exerciseRepo;
        }

        public async Task<Set> GetByIdAsync(Guid setId, CancellationToken ct = default)
        {
            return await _repo.GetByIdAsync(setId, ct);
        }

        public async Task<List<Set>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
        {
            return await _repo.GetAllAsync(page, pageSize, ct);
        }

        public async Task<Set?> CreateAsync(CreateSetDto dto, CancellationToken ct = default)
        {
            var sets = await _repo.GetAllByWorkoutExerciseIdAsync(dto.WorkoutExerciseId, ct);
            if (sets != null && sets.Count > 0)
            {
                var setNumber = sets.Max(s => s.SetNumber) + 1;
                var set = new Set(setNumber, dto.Repetitions, dto.Weight, dto.WorkoutExerciseId);
                return await _repo.AddAsync(set, ct);
            }
            else
            {
                var set = new Set(1, dto.Repetitions, dto.Weight, dto.WorkoutExerciseId);
                return await _repo.AddAsync(set, ct);
            }
        }

        public async Task<Set?> UpdateSetAsync(Guid setId, UpdateSetDto dto, CancellationToken ct = default)
        {
            var set = await _repo.GetByIdAsync(setId, ct);
            if (set == null)
            {
                return null;
            }
    
            set.Repetitions = dto.Repetitions;
            set.Weight = dto.Weight;
            
            return await _repo.UpdateAsync(set, ct);
        }

        public async Task<bool> DeleteAsync(Guid setId, CancellationToken ct = default)
        {
            var set = await _repo.GetByIdAsync(setId, ct);
            if (set == null)
            {
                return false;
            }

            await _repo.DeleteAsync(set, ct);
            return true;
        }
    }
}
