using Base.Repositories;

namespace Sets
{
    public interface ISetRepository : IRepository<Set, Guid>
    {
        Task<List<Set>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);
        Task<List<Set>> GetAllByWorkoutExerciseIdAsync(Guid workoutExerciseId, CancellationToken ct = default);
        Task<Set> GetByIdAsync(Guid setId, CancellationToken ct = default);
        Task<Set> AddAsync(Set entity, CancellationToken ct = default);
        Task<Set> UpdateAsync(Set entity, CancellationToken ct = default);
    }
}
