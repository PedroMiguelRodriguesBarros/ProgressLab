using Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Sets
{
    public class SetRepository : ISetRepository
    {
        private readonly ProgressLabDbContext _dbContext;

        public SetRepository(ProgressLabDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private IQueryable<Set> QueryWithRelations()
        {
            return _dbContext.Sets;
        }

        public async Task<Set> AddAsync(Set set, CancellationToken ct = default)
        {
            _dbContext.Sets.Add(set);
            await _dbContext.SaveChangesAsync(ct);

            return set;
        }

        public async Task<Set?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await QueryWithRelations()
                .AsNoTracking()
                .FirstOrDefaultAsync(it => it.SetId == id, ct);
        }

        public async Task<IReadOnlyList<Set>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
        {
            if (page < 1)
                throw new ArgumentOutOfRangeException(nameof(page));

            if (pageSize < 1 || pageSize > 100)
            throw new ArgumentOutOfRangeException(nameof(pageSize));

            return await QueryWithRelations()
                .AsNoTracking()
                .OrderBy(it => it.SetNumber)
                .ThenBy(s => s.SetId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
        }

        public async Task<bool> DeleteAsync(Set set, CancellationToken ct = default)
        {
            _dbContext.Sets.Remove(set);
            var affectedRows = await _dbContext.SaveChangesAsync(ct);

            return affectedRows > 0;
        }

        public async Task<List<Set>> GetAllByWorkoutExerciseIdAsync(Guid workoutExerciseId, CancellationToken ct = default)
        {
            return await QueryWithRelations()
                .AsNoTracking()
                .Where(it => it.WorkoutExerciseId == workoutExerciseId)
                .OrderByDescending(it => it.SetNumber)
                .ToListAsync(ct);
        }

        public async Task UpdateAsync(Set set, CancellationToken ct = default)
        {
            _dbContext.Sets.Update(set);
            await _dbContext.SaveChangesAsync(ct);

        }

        public async Task<bool> ExistsAsync(Guid setId, CancellationToken ct = default)
        {
            return await _dbContext.Sets
                .AnyAsync(it => it.SetId == setId, ct);
        }
    }
}
