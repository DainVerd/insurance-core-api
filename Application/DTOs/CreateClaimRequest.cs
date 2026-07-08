namespace Application.DTOs;

public class CreateClaimRequest
{
    public Guid PolicyId { get; set; }
    public DateTimeOffset IncidentDate { get; set; }
    public decimal AmountRequested { get; set; }
    public string? DecisionReason = null;
}
