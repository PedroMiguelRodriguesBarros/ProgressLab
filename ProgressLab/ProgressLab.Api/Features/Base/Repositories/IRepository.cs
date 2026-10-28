namespace Base.Repositories
{
    public interface IRepository<T, TId>
    {
        Task<T> AddAsync(T entity, CancellationToken ct = default);
        Task DeleteAsync(T entity, CancellationToken ct = default);
        Task<T> GetByIdAsync(TId id, CancellationToken ct = default);
        Task<List<T>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);
    }
}