using Application.Interfaces.Repositories;
using Domain.Interfaces;
using System.Collections.Concurrent;

namespace Infrastructure.Repositories;

public abstract class BaseInMemoryRepository<T> : IRepository<T> where T : class, IEntity
{
    protected readonly ConcurrentDictionary<Guid, T> Storage = new();

    public T? GetById(Guid id)
    {
        Storage.TryGetValue(id, out var entity);

        return entity;
    }

    public void Add(T entity)
    {
        if (!Storage.TryAdd(entity.Id, entity))
            throw new InvalidOperationException($"Entity {typeof(T).Name} with Id {entity.Id} already exists.");
    }

    public void Update(T entity)
    {
        Storage[entity.Id] = entity;
    }
}
