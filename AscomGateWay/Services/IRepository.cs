namespace AscomPayPG.Services
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> GetOne(long itemId);
        Task<T> Create(T item);
        Task<bool> Update(T item, long itemId);
        Task<bool> Delete(long itemId);

    }
}