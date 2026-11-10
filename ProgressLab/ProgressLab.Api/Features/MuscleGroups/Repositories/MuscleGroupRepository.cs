using Base.Repositories;
using Data.Context;
using Microsoft.EntityFrameworkCore;

namespace MuscleGroups
{
    public class MuscleGroupRepository : IMuscleGroupRepository
    {
        private readonly ProgressLabDbContext _dbContext;

        public MuscleGroupRepository(ProgressLabDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<MuscleGroup> AddAsync(MuscleGroup muscleGroup, CancellationToken ct = default)
        {
            _dbContext.MuscleGroups.Add(muscleGroup);
            await _dbContext.SaveChangesAsync(ct);

            return muscleGroup;
        }

        public async Task<MuscleGroup?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _dbContext.MuscleGroups
                .AsNoTracking()
                .FirstOrDefaultAsync(it => it.MuscleGroupId == id, ct);

        }

        public async Task<IReadOnlyList<MuscleGroup>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
        {
            if (page < 1)
                throw new ArgumentOutOfRangeException(nameof(page));

            if (pageSize < 1 || pageSize > 100)
                throw new ArgumentOutOfRangeException(nameof(pageSize));

            return await _dbContext.MuscleGroups
                .AsNoTracking()
                .OrderBy(mg => mg.Name)
                .ThenBy(mg => mg.MuscleGroupId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
        }

        public async Task UpdateAsync(MuscleGroup muscleGroup, CancellationToken ct = default)
        {
            _dbContext.Entry(muscleGroup).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync(ct);
        }

        public async Task<bool> DeleteAsync(MuscleGroup entity, CancellationToken ct = default)
        {
            _dbContext.MuscleGroups.Remove(entity);
            var affectedRows = await _dbContext.SaveChangesAsync(ct);

            return affectedRows > 0;
        }
        public async Task<bool> ExistsAsync(Guid muscleGroupId, CancellationToken ct = default)
        {
            return await _dbContext.MuscleGroups
                .AnyAsync(it => it.MuscleGroupId == muscleGroupId, ct);
        }
    }
}

