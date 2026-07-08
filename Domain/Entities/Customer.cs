using Domain.Interfaces;

namespace Domain.Entities;

public class Customer : IEntity
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
}
