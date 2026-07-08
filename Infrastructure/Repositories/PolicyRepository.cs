using Application.Interfaces.Repositories;
using Domain.Entities;

namespace Infrastructure.Repositories;

public class PolicyRepository : BaseInMemoryRepository<Policy>, IPolicyRepository
{
}
