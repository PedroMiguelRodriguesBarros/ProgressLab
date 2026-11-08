using MuscleGroups;
using ExercisesMuscleGroups;

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

        public async Task<Exercise?> CreateAsync(CreateExerciseDto dto, CancellationToken ct = default)
        {
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

                if (muscleGroupDto.Role == MuscleGroupRole.Primary)
                {
                    if (muscleGroups.Any(mg => mg.Role == MuscleGroupRole.Primary)) //Se já existir um grupo muscular primário, não permitir adicionar outro
                    {
                        return null; // Only one primary muscle group allowed
                    }
                }
                muscleGroups.Add(exerciseMuscleGroup);
            }
            
            var exercise = new Exercise(dto.Name, dto.Description);
            exercise.AddMuscleGroups(muscleGroups);
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

        public async Task<Exercise?> UpdateExerciseAsync(Guid exerciseId, UpdateExerciseDto dto, CancellationToken ct = default)
        {
            var exercise = await _repo.GetByIdAsync(exerciseId, ct);
            if (exercise == null)
            {
                return null;
            }

            exercise.Name = dto.Name;
            exercise.Description = dto.Description;

            return await _repo.UpdateAsync(exercise, ct);
        }
    }
}
