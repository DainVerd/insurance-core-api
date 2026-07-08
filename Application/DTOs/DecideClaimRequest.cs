using Domain.Constants;

namespace Application.DTOs;

public class DecideClaimRequest
{
    public ClaimStatus Status { get; set; }
    public string? DecisionReason { get; set; }
}
