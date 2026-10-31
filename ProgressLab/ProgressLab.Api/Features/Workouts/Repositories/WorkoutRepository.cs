using Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Workouts
{
    public class WorkoutRepository : IWorkoutRepository
    {
        private readonly WorkoutsDbContext _dbContext;

        public WorkoutRepository(WorkoutsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private IQueryable<Workout> QueryWithRelations()
        {
            return _dbContext.Workouts;
        }

        public async Task<Workout> AddAsync(Workout workout, CancellationToken ct = default)
        {
            _dbContext.Workouts.Add(workout);
            await _dbContext.SaveChangesAsync(ct);

            var createdWorkout = await QueryWithRelations()
                .AsNoTracking()
                .FirstOrDefaultAsync(it => it.WorkoutId == workout.WorkoutId, ct);

            return createdWorkout!;
        }

        public async Task DeleteAsync(Workout workout, CancellationToken ct = default)
        {
            var existingWorkout = await _dbContext.Workouts.FirstOrDefaultAsync(it => it.WorkoutId == workout.WorkoutId, ct);
            if (existingWorkout != null)
            {
                _dbContext.Workouts.Remove(existingWorkout);
                await _dbContext.SaveChangesAsync(ct);
            }
        }

        public async Task<List<Workout>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
        {
            return await QueryWithRelations()
                .AsNoTracking()
                .OrderByDescending(it => it.Date)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
        }

        public async Task<Workout> UpdateAsync(Workout workout, CancellationToken ct = default)
        {
            _dbContext.Workouts.Update(workout);
            await _dbContext.SaveChangesAsync(ct);

            return workout;
        }

        public async Task<Workout> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var workout = await QueryWithRelations()
                .AsNoTracking()
                .FirstOrDefaultAsync(it => it.WorkoutId == id, ct);

            return workout!;
        }

        public async Task<bool> ExistsAsync(Guid workoutId, CancellationToken ct = default)
        {
            return await _dbContext.Workouts
                .AsNoTracking()
                .AnyAsync(it => it.WorkoutId == workoutId, ct);
        }
    }
}
