using System.Collections.Concurrent;

namespace Restaurant.IdempotentLibrary.Repositories
{
    public class InMemoryRepository<T> : IInMemoryRepository<T> where T : class
    {
        private readonly ConcurrentBag<T> _repo = new ConcurrentBag<T>();

        public void AddOrUpdate(T entity)
        {
            _repo.Add(entity);
        }

        public IEnumerable<T> Get()
        {
            return _repo;
        }

        public void Initialize()
        {
            _repo.Clear();
        }
    }
}
