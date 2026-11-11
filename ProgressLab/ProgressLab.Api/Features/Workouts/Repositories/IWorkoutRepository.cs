using Base.Repositories;

namespace Workouts
{
    public interface IWorkoutRepository : IRepository<Workout, Guid>
    {
        Task<bool> ExistsAsync(Guid workoutId, CancellationToken ct = default);
        Task<Workout?> GetByIdForUpdateAsync(Guid workoutId, CancellationToken ct = default);
        Task<WorkoutExercise?> GetWorkoutExerciseByIdAsync(Guid workoutExerciseId, CancellationToken ct = default);
    }
}
