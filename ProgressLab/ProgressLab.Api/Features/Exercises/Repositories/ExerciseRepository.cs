using Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Exercises
{
    public class ExerciseRepository : IExerciseRepository
    {
        private readonly ProgressLabDbContext _dbContext;

        public ExerciseRepository(ProgressLabDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private IQueryable<Exercise> QueryWithRelations()
        {
            return _dbContext.Exercises
            .Include(e => e.ExerciseMuscleGroups)
            .ThenInclude(emg => emg.MuscleGroup);
        }

        public async Task<Exercise> AddAsync(Exercise exercise, CancellationToken ct = default)
        {
            _dbContext.Exercises.Add(exercise);
            await _dbContext.SaveChangesAsync(ct);

            return await QueryWithRelations()
                .AsNoTracking()
                .FirstAsync(e => e.ExerciseId == exercise.ExerciseId, ct);
        }

        public async Task<Exercise?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await QueryWithRelations()
                .AsNoTracking()
                .FirstOrDefaultAsync(it => it.ExerciseId == id, ct);
        }

        public async Task<IReadOnlyList<Exercise>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
        {
            if (page < 1)
            throw new ArgumentOutOfRangeException(nameof(page));

            if (pageSize < 1 || pageSize > 100)
            throw new ArgumentOutOfRangeException(nameof(pageSize));

            return await QueryWithRelations()
                .AsNoTracking()
                .OrderBy(e => e.Name)
                .ThenBy(e => e.ExerciseId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
        }

        public async Task<Exercise?> GetByIdForUpdateAsync(Guid id, CancellationToken ct = default)
        {
            return await QueryWithRelations()
            .FirstOrDefaultAsync(e => e.ExerciseId == id, ct);
        }

        public async Task UpdateAsync(Exercise exercise, CancellationToken ct = default)
        {
            await _dbContext.SaveChangesAsync(ct);
        }

        public async Task<bool> DeleteAsync(Exercise exercise, CancellationToken ct = default)
        {
            _dbContext.Exercises.Remove(exercise);
            var affectedRows = await _dbContext.SaveChangesAsync(ct);

            return affectedRows > 0;
        }

        public async Task<bool> ExistsAsync(Guid exerciseId, CancellationToken ct = default)
        {
            return await _dbContext.Exercises
                .AnyAsync(it => it.ExerciseId == exerciseId, ct);
        }
    }
}
