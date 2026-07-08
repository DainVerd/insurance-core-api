using Domain.Constants;

namespace Application.DTOs;

public class ClaimDto
{
    public Guid Id { get; set; }
    public Guid PolicyId { get; set; }
    public DateTimeOffset IncidentDate { get; set; }
    public decimal AmountRequested { get; set; }
    public ClaimStatus Status { get; set; }
    public string? DecisionReason { get; set; }
}
