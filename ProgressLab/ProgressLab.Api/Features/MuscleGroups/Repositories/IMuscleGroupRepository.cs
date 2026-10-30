using Base.Repositories;

namespace MuscleGroups
{
    public interface IMuscleGroupRepository : IRepository<MuscleGroup, Guid>
    {
        Task<List<MuscleGroup>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);
        Task<MuscleGroup> GetByIdAsync(Guid muscleGroupId, CancellationToken ct = default);
        Task<MuscleGroup> AddAsync(MuscleGroup entity, CancellationToken ct = default);
        Task<MuscleGroup> UpdateAsync(MuscleGroup entity, CancellationToken ct = default);
    }
}
