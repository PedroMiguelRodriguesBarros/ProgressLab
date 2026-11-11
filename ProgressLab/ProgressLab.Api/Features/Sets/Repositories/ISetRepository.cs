using Base.Repositories;

namespace Sets
{
    public interface ISetRepository : IRepository<Set, Guid>
    {
        Task<bool> ExistsAsync(Guid setId, CancellationToken ct = default);
        Task<List<Set>> GetAllByWorkoutExerciseIdAsync(Guid workoutExerciseId, CancellationToken ct = default);

    }
}
