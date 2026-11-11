using Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Workouts
{
    public class WorkoutRepository : IWorkoutRepository
    {
        private readonly ProgressLabDbContext _dbContext;

        public WorkoutRepository(ProgressLabDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private IQueryable<Workout> QueryWithRelations()
        {
            return _dbContext.Workouts
                .Include(w => w.WorkoutExercises)
                    .ThenInclude(we => we.Exercise)
                .Include(w => w.WorkoutExercises)
                    .ThenInclude(we => we.Sets)
                .AsSplitQuery();
        }

        public async Task<Workout> AddAsync(Workout workout, CancellationToken ct = default)
        {
            _dbContext.Workouts.Add(workout);
            await _dbContext.SaveChangesAsync(ct);

            return workout;
        }

        public async Task<Workout?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await QueryWithRelations()
                .AsNoTracking()
                .FirstOrDefaultAsync(it => it.WorkoutId == id, ct);
        }

        public async Task<Workout?> GetByIdForUpdateAsync(Guid workoutId, CancellationToken ct = default)
        {
            return await _dbContext.Workouts
                .Include(w => w.WorkoutExercises)
                .FirstOrDefaultAsync(it => it.WorkoutId == workoutId, ct);
        }

        public async Task<WorkoutExercise?> GetWorkoutExerciseByIdAsync(Guid workoutExerciseId, CancellationToken ct = default)
        {
            return await _dbContext.WorkoutExercises
                .Include(we => we.Sets)
                .FirstOrDefaultAsync(we => we.WorkoutExerciseId == workoutExerciseId, ct);
        }

        public async Task<IReadOnlyList<Workout>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
        {
            if (page < 1)
                throw new ArgumentOutOfRangeException(nameof(page));

            if (pageSize < 1 || pageSize > 100)
            throw new ArgumentOutOfRangeException(nameof(pageSize));

            return await QueryWithRelations()
                .AsNoTracking()
                .OrderByDescending(it => it.Date)
                .ThenBy(w => w.WorkoutId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
        }

        public async Task UpdateAsync(Workout workout, CancellationToken ct = default)
        {
            await _dbContext.SaveChangesAsync(ct);
        }

        public async Task<bool> DeleteAsync(Workout workout, CancellationToken ct = default)
        {
            _dbContext.Workouts.Remove(workout);
            var affectedRows = await _dbContext.SaveChangesAsync(ct);

            return affectedRows > 0;
        }

        public async Task<bool> ExistsAsync(Guid workoutId, CancellationToken ct = default)
        {
            return await _dbContext.Workouts
                .AnyAsync(it => it.WorkoutId == workoutId, ct);
        }
    }
}
