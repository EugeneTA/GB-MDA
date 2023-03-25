namespace Restaurant.IdempotentLibrary.Repositories
{
    public interface IInMemoryRepository<T> where T : class
    {
        public void AddOrUpdate(T entity);
        public IEnumerable<T> Get();
        public void Initialize();

    }
}
