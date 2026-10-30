using Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Exercises
{
    public class ExerciseRepository : IExerciseRepository
    {
        private readonly ExercisesDbContext _dbContext;

        public ExerciseRepository(ExercisesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private IQueryable<Exercise> QueryWithRelations()
        {
            return _dbContext.Exercises;
        }

        public async Task<Exercise> AddAsync(Exercise exercise, CancellationToken ct = default)
        {
            _dbContext.Exercises.Add(exercise);
            await _dbContext.SaveChangesAsync(ct);

            var createdExercise = await QueryWithRelations()
                .AsNoTracking()
                .FirstOrDefaultAsync(it => it.ExerciseId == exercise.ExerciseId, ct);

            return createdExercise!;
        }

        public async Task DeleteAsync(Exercise exercise, CancellationToken ct = default)
        {
            var existingExercise = await _dbContext.Exercises.FirstOrDefaultAsync(it => it.ExerciseId == exercise.ExerciseId, ct);
            if (existingExercise != null)
            {
                _dbContext.Exercises.Remove(existingExercise);
                await _dbContext.SaveChangesAsync(ct);
            }
        }

        public async Task<List<Exercise>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
        {
            return await QueryWithRelations()
                .AsNoTracking()
                .OrderByDescending(it => it.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
        }

        public async Task<Exercise> UpdateAsync(Exercise exercise, CancellationToken ct = default)
        {
            _dbContext.Exercises.Update(exercise);
            await _dbContext.SaveChangesAsync(ct);

            return exercise;
        }

        public async Task<Exercise> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var exercise = await QueryWithRelations()
                .AsNoTracking()
                .FirstOrDefaultAsync(it => it.ExerciseId == id, ct);

            return exercise!;
        }
    }
}
