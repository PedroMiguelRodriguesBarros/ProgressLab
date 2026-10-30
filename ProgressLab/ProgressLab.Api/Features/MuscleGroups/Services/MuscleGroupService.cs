namespace MuscleGroups
{
    public class MuscleGroupService
    {
        private readonly IMuscleGroupRepository _repo;

        public MuscleGroupService(IMuscleGroupRepository repo)
        {
            _repo = repo;
        }

        public async Task<MuscleGroup> GetByIdAsync(Guid muscleGroupId, CancellationToken ct = default)
        {
            return await _repo.GetByIdAsync(muscleGroupId, ct);
        }

        public async Task<List<MuscleGroup>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
        {
            return await _repo.GetAllAsync(page, pageSize, ct);
        }

        public async Task<MuscleGroup?> CreateAsync(string name, string description, CancellationToken ct = default)
        {
            var muscleGroup = new MuscleGroup(name, description);
            return await _repo.AddAsync(muscleGroup, ct);
        }

        public async Task<MuscleGroup?> UpdateMuscleGroupAsync(Guid muscleGroupId, UpdateMuscleGroupDto dto, CancellationToken ct = default)
        {
            var muscleGroup = await _repo.GetByIdAsync(muscleGroupId, ct);
            if (muscleGroup == null)
            {
                return null;
            }

            muscleGroup.Name = dto.Name;
            muscleGroup.Description = dto.Description;
     
            return await _repo.UpdateAsync(muscleGroup, ct);
        }

        public async Task DeleteAsync(Guid muscleGroupId, CancellationToken ct = default)
        {
            var muscleGroup = await _repo.GetByIdAsync(muscleGroupId, ct);
            if (muscleGroup != null)
            {
                await _repo.DeleteAsync(muscleGroup, ct);
            }
        }
    }
}
