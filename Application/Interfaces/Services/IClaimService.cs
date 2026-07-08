using Application.DTOs;

namespace Application.Interfaces.Services;

public interface IClaimService
{
    Guid CreateClaim(CreateClaimRequest claimToAdd);
    ClaimDto GetById(Guid id);
}
