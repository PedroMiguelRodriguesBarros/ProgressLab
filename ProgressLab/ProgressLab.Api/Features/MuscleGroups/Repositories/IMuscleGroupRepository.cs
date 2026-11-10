using Base.Repositories;

namespace MuscleGroups
{
    public interface IMuscleGroupRepository : IRepository<MuscleGroup, Guid>
    {
        Task<bool> ExistsAsync(Guid muscleGroupId, CancellationToken ct = default);
    }
}
