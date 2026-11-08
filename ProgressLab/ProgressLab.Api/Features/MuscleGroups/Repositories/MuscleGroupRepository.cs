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

        private IQueryable<MuscleGroup> QueryWithRelations()
        {
            return _dbContext.MuscleGroups;
        }

        public async Task<MuscleGroup> AddAsync(MuscleGroup muscleGroup, CancellationToken ct = default)
        {
            _dbContext.MuscleGroups.Add(muscleGroup);
            await _dbContext.SaveChangesAsync(ct);

            var createdMuscleGroup = await QueryWithRelations()
                .AsNoTracking()
                .FirstOrDefaultAsync(it => it.MuscleGroupId == muscleGroup.MuscleGroupId, ct);

            return createdMuscleGroup!;
        }

        public async Task DeleteAsync(MuscleGroup muscleGroup, CancellationToken ct = default)
        {
            var existingMuscleGroup = await _dbContext.MuscleGroups.FirstOrDefaultAsync(it => it.MuscleGroupId == muscleGroup.MuscleGroupId, ct);
            if (existingMuscleGroup != null)
            {
                _dbContext.MuscleGroups.Remove(existingMuscleGroup);
                await _dbContext.SaveChangesAsync(ct);
            }
        }

        public async Task<List<MuscleGroup>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
        {
            return await QueryWithRelations()
                .AsNoTracking()
                .OrderByDescending(it => it.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
        }

        public async Task<MuscleGroup> UpdateAsync(MuscleGroup muscleGroup, CancellationToken ct = default)
        {
            _dbContext.MuscleGroups.Update(muscleGroup);
            await _dbContext.SaveChangesAsync(ct);

            return muscleGroup;
        }

        public async Task<MuscleGroup> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var muscleGroup = await QueryWithRelations()
                .AsNoTracking()
                .FirstOrDefaultAsync(it => it.MuscleGroupId == id, ct);

            return muscleGroup!;
        }

        public async Task<bool> ExistsAsync(Guid muscleGroupId, CancellationToken ct = default)
        {
            return await _dbContext.MuscleGroups
                .AsNoTracking()
                .AnyAsync(it => it.MuscleGroupId == muscleGroupId, ct);
        }
    }
}
