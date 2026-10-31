using Base.Repositories;

namespace Workouts
{
    public interface IWorkoutRepository : IRepository<Workout, Guid>
    {
        Task<List<Workout>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);
        Task<Workout> GetByIdAsync(Guid workoutId, CancellationToken ct = default);
        Task<Workout> AddAsync(Workout entity, CancellationToken ct = default);
        Task<Workout> UpdateAsync(Workout entity, CancellationToken ct = default);
    }
}
