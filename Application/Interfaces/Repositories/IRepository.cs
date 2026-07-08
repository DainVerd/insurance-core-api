namespace Application.Interfaces.Repositories;

public interface IRepository<T> where T : class
{
    T? GetById(Guid id);
    void Add(T entity);
    void Update(T entity);
    void Delete(Guid id);
}
