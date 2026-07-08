using Application.Interfaces.Repositories;
using Domain.Entities;

namespace Infrastructure.Repositories;

public class ClaimRepository : BaseInMemoryRepository<Claim>, IClaimRepository
{
}
