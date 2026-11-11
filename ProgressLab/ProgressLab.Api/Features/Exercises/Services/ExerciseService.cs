using MuscleGroups;
using ExercisesMuscleGroups;

namespace Exercises
{
    public class ExerciseService
    {
        private readonly IExerciseRepository _repo;
        private readonly IMuscleGroupRepository _muscleGroupRepo;
        private readonly IMapper<Exercise, ExerciseDetailedDto> _detailedMapper;
        private readonly IMapper<Exercise, ExerciseSummaryDto> _summaryMapper;
        private readonly IMapper<CreateExerciseDto, Exercise> _createMapper;
        public ExerciseService(IExerciseRepository repo, IMuscleGroupRepository muscleGroupRepo, IMapper<Exercise, ExerciseDetailedDto> detailedMapper, IMapper<Exercise, ExerciseSummaryDto> summaryMapper, IMapper<CreateExerciseDto, Exercise> createMapper)
        {
            _repo = repo;
            _muscleGroupRepo = muscleGroupRepo;
            _detailedMapper = detailedMapper;
            _summaryMapper = summaryMapper;
            _createMapper = createMapper;
        }

        public async Task<ExerciseDetailedDto?> GetByIdAsync(Guid exerciseId, CancellationToken ct = default)
        {
            var exercise = await _repo.GetByIdAsync(exerciseId, ct);
            if (exercise == null)
            {
                return null;
            }

            return _detailedMapper.Map(exercise);
        }

        public async Task<List<ExerciseSummaryDto>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
        {
            var exercises = await _repo.GetAllAsync(page, pageSize, ct);
            return exercises.Select(_summaryMapper.Map).ToList();
        }

        public async Task<ExerciseDetailedDto?> CreateAsync(CreateExerciseDto dto, CancellationToken ct = default)
        {
            if (!HasValidMuscleGroups(dto.MuscleGroups))
            {
                return null;
            }
            
            var muscleGroups = new List<ExerciseMuscleGroup>();
            
            foreach (var muscleGroupDto in dto.MuscleGroups)
            {
                var muscleGroup = await _muscleGroupRepo.GetByIdAsync(muscleGroupDto.MuscleGroupId, ct);
                if (muscleGroup == null)
                {
                    return null;
                }

                var exerciseMuscleGroup = new ExerciseMuscleGroup
                {
                    MuscleGroupId = muscleGroup.MuscleGroupId,
                    Role = muscleGroupDto.Role
                };

                muscleGroups.Add(exerciseMuscleGroup);
            }
            
            var exercise = _createMapper.Map(dto);
            exercise.AddMuscleGroups(muscleGroups);
            var createdExercise = await _repo.AddAsync(exercise, ct);
          
            return _detailedMapper.Map(createdExercise);
        }

        public async Task<ExerciseDetailedDto?> UpdateAsync(Guid exerciseId, UpdateExerciseDto dto, CancellationToken ct = default)
        {
            if (!HasValidMuscleGroups(dto.MuscleGroups))
            {
                return null;
            }

            var exercise = await _repo.GetByIdForUpdateAsync(exerciseId, ct);

            if (exercise == null)
            {
                return null;
            }

            foreach (var muscleGroupDto in dto.MuscleGroups)
            {
                var exists = await _muscleGroupRepo.GetByIdAsync(muscleGroupDto.MuscleGroupId, ct);

                if (exists == null)
                {
                    return null;
                }
            }

            exercise.Name = dto.Name.Trim();
            exercise.Description = dto.Description.Trim();

            var requestedGroups = dto.MuscleGroups.ToDictionary(
                mg => mg.MuscleGroupId
            );

            foreach (var existingGroup in exercise.ExerciseMuscleGroups.ToList())
            {
                if (requestedGroups.TryGetValue(existingGroup.MuscleGroupId, out var requestedGroup))
                {
                    existingGroup.Role = requestedGroup.Role;

                    requestedGroups.Remove(existingGroup.MuscleGroupId);
                }
                else
                {
                    exercise.ExerciseMuscleGroups.Remove(existingGroup);
                }
            }

            foreach (var newGroup in requestedGroups.Values)
            {
                exercise.ExerciseMuscleGroups.Add(new ExerciseMuscleGroup
                {
                    ExerciseId = exercise.ExerciseId,
                    Exercise = exercise,
                    MuscleGroupId = newGroup.MuscleGroupId,
                    Role = newGroup.Role
                });
            }

            await _repo.UpdateAsync(exercise, ct);

            var updatedExercise = await _repo.GetByIdAsync(exerciseId, ct);

            return updatedExercise == null
                ? null
                : _detailedMapper.Map(updatedExercise);
        }

        public async Task<bool> DeleteAsync(Guid exerciseId, CancellationToken ct = default)
        {
            var exercise = await _repo.GetByIdAsync(exerciseId, ct);
            if (exercise == null)
            {
                return false;
            }

            return await _repo.DeleteAsync(exercise, ct);
        }

        private static bool HasValidMuscleGroups(IReadOnlyCollection<ExerciseMuscleGroupInputDto> muscleGroups)
        {
            if (muscleGroups.Count == 0)
                return false; // No muscle groups provided

            if (muscleGroups.Count(mg => mg.Role == MuscleGroupRole.Primary) != 1)
                return false; // Must have exactly one primary muscle group

            if (muscleGroups.Select(mg => mg.MuscleGroupId).Distinct().Count() != muscleGroups.Count)
                return false; // Duplicate muscle groups

            if (muscleGroups.Any(mg => mg.Role != MuscleGroupRole.Primary && mg.Role != MuscleGroupRole.Secondary))
                return false; // Invalid role provided

            return true;
        }
    }
}
