using Domain.Constants;

namespace Application.DTOs;

public class CreatePolicyRequest
{
    public Guid CustomerId { get; set; }
    public PolicyProductType ProductType { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal Premium { get; set; }
}
