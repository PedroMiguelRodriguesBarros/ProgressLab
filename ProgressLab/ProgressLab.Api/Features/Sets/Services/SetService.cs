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

        public async Task<Set?> CreateAsync(int reps, int sets, float weight, Guid exerciseId, CancellationToken ct = default)
        {
            var exercise = await _exerciseRepo.GetByIdAsync(exerciseId, ct);
            if (exercise == null)
            {
                return null;
            }

            var set = new Set(sets, reps, weight, exercise);
            return await _repo.AddAsync(set, ct);
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
    }
}
