using Workouts;
namespace Sets
{
    public class SetService
    {
        private readonly ISetRepository _repo;
        private readonly IWorkoutRepository _workoutRepo;
        private readonly IMapper<Set, SetSummaryDto> _summaryMapper;
        private readonly IMapper<Set, SetDetailedDto> _detailedMapper;

        public SetService(ISetRepository repo, IWorkoutRepository workoutRepo, IMapper<Set, SetSummaryDto> summaryMapper, IMapper<Set, SetDetailedDto> detailedMapper)
        {
            _repo = repo;
            _workoutRepo = workoutRepo;
            _summaryMapper = summaryMapper;
            _detailedMapper = detailedMapper;
        }

        public async Task<SetDetailedDto> GetByIdAsync(Guid setId, CancellationToken ct = default)
        {
            var set = await _repo.GetByIdAsync(setId, ct);
            if (set == null)
            {
                throw new KeyNotFoundException($"Set with ID {setId} not found.");
            }
            return _detailedMapper.Map(set);
        }

        public async Task<IReadOnlyList<SetSummaryDto>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
        {
            var sets = await _repo.GetAllAsync(page, pageSize, ct);
            return sets.Select(_summaryMapper.Map).ToList();
        }

        public async Task<IReadOnlyList<SetSummaryDto>> GetAllByWorkoutExerciseIdAsync(Guid workoutExerciseId, CancellationToken ct = default)
        {   
            var workoutExercise = await _workoutRepo.GetWorkoutExerciseByIdAsync(
                workoutExerciseId, ct);

            if (workoutExercise == null)
            {
                throw new KeyNotFoundException($"Workout exercise with ID {workoutExerciseId} not found.");
            }

            return workoutExercise.Sets
                .OrderBy(s => s.SetNumber)
                .Select(_summaryMapper.Map)
                .ToList();
        }

        public async Task<SetDetailedDto> CreateAsync(Guid workoutExerciseId, CreateSetDto dto, CancellationToken ct = default)
        {
            var workoutExercise = await _workoutRepo.GetWorkoutExerciseByIdAsync(workoutExerciseId, ct);
            if (workoutExercise == null)
            {
                throw new KeyNotFoundException($"Workout exercise with ID {workoutExerciseId} not found.");
            }

            var setNumber = workoutExercise.Sets
                .Select(s => s.SetNumber)
                .DefaultIfEmpty(0)
                .Max() + 1;

            var set = new Set(
                setNumber,
                dto.Repetitions,
                dto.Weight
            );

            set.WorkoutExerciseId = workoutExerciseId;
            workoutExercise.Sets.Add(set);

            await _repo.AddAsync(set, ct);

            return _detailedMapper.Map(set);
        }

        public async Task<SetDetailedDto?> UpdateSetAsync(Guid setId, UpdateSetDto dto, CancellationToken ct = default)
        {
            var set = await _repo.GetByIdAsync(setId, ct);
            if (set == null)
            {
                return null;
            }
    
            set.Repetitions = dto.Repetitions;
            set.Weight = dto.Weight;
            
            await _repo.UpdateAsync(set, ct);

            return _detailedMapper.Map(set);
        }

        public async Task<bool> DeleteAsync(Guid setId, CancellationToken ct = default)
        {
            var existingSet = await _repo.GetByIdAsync(setId, ct);
            if (existingSet == null)
            {
                return false;
            }

            var workoutExercise = await _workoutRepo.GetWorkoutExerciseByIdAsync(
                existingSet.WorkoutExerciseId, ct);

            if (workoutExercise == null)
            {
                return false;
            }

            var set = workoutExercise.Sets
                .FirstOrDefault(s => s.SetId == setId);

            if (set == null)
            {
                return false;
            }

            workoutExercise.Sets.Remove(set);

            var remainingSets = workoutExercise.Sets
                .OrderBy(s => s.SetNumber)
                .ThenBy(s => s.SetId)
                .ToList();

            for (int i = 0; i < remainingSets.Count; i++)
            {
                remainingSets[i].SetNumber = i + 1;
            }

            return await _repo.DeleteAsync(set, ct);
        }
    }
}
