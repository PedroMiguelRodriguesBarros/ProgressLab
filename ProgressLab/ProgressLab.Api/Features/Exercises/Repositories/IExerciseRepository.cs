using Base.Repositories;

namespace Exercises
{
    public interface IExerciseRepository : IRepository<Exercise, Guid>
    {
        Task<bool> ExistsAsync(Guid exerciseId, CancellationToken ct = default);
        Task<Exercise?> GetByIdForUpdateAsync(Guid id, CancellationToken ct = default);
    }
}
