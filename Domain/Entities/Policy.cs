using Domain.Constants;
using Domain.Interfaces;

namespace Domain.Entities;

public class Policy : IEntity
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public PolicyProductType ProductType { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal Premium { get; set; }
    public PolicyStatus Status { get; set; }
}
