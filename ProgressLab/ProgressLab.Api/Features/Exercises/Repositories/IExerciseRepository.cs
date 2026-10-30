using Base.Repositories;

namespace Exercises
{
    public interface IExerciseRepository : IRepository<Exercise, Guid>
    {
        Task<List<Exercise>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);
        Task<Exercise> GetByIdAsync(Guid exerciseId, CancellationToken ct = default);
        Task<Exercise> AddAsync(Exercise entity, CancellationToken ct = default);
        Task<Exercise> UpdateAsync(Exercise entity, CancellationToken ct = default);
    }
}
