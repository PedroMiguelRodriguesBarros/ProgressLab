namespace Base.Repositories
{
    public interface IRepository<TEntity, TId>
    {
        Task<TEntity> AddAsync(TEntity entity, CancellationToken ct = default);
        Task<TEntity?> GetByIdAsync(TId id, CancellationToken ct = default);
        Task<IReadOnlyList<TEntity>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);
        Task UpdateAsync(TEntity entity, CancellationToken ct = default);
        Task<bool> DeleteAsync(TEntity entity, CancellationToken ct = default);
    }
}