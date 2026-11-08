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

            var createdSet = await QueryWithRelations()
                .AsNoTracking()
                .FirstOrDefaultAsync(it => it.SetId == set.SetId, ct);

            return createdSet!;
        }

        public async Task DeleteAsync(Set set, CancellationToken ct = default)
        {
            var existingSet = await _dbContext.Sets.FirstOrDefaultAsync(it => it.SetId == set.SetId, ct);
            if (existingSet != null)
            {
                _dbContext.Sets.Remove(existingSet);
                await _dbContext.SaveChangesAsync(ct);
            }
        }

        public async Task<List<Set>> GetAllByWorkoutExerciseIdAsync(Guid workoutExerciseId, CancellationToken ct = default)
        {
            return await QueryWithRelations()
                .AsNoTracking()
                .Where(it => it.WorkoutExerciseId == workoutExerciseId)
                .OrderByDescending(it => it.SetNumber)
                .ToListAsync(ct);
        }

        public async Task<List<Set>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
        {
            return await QueryWithRelations()
                .AsNoTracking()
                .OrderByDescending(it => it.SetNumber)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
        }

        public async Task<Set> UpdateAsync(Set set, CancellationToken ct = default)
        {
            _dbContext.Sets.Update(set);
            await _dbContext.SaveChangesAsync(ct);

            return set;
        }

        public async Task<Set> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var set = await QueryWithRelations()
                .AsNoTracking()
                .FirstOrDefaultAsync(it => it.SetId == id, ct);

            return set!;
        }

        public async Task<bool> ExistsAsync(Guid setId, CancellationToken ct = default)
        {
            return await _dbContext.Sets
                .AsNoTracking()
                .AnyAsync(it => it.SetId == setId, ct);
        }
    }
}
