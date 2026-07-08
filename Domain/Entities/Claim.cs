using Domain.Constants;
using Domain.Interfaces;

namespace Domain.Entities;

public class Claim : IEntity
{
    public Guid Id { get; set; }
    public Guid PolicyId { get; set; }
    public DateTimeOffset IncidentDate { get; set; }
    public decimal AmountRequested { get; set; }
    public ClaimStatus Status { get; set; }
    public string? DecisionReason = null;
}
