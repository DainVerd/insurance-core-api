using Domain.Constants;
using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IPolicyRepository : IRepository<Policy>
{
    IEnumerable<Policy> GetActivePoliciesByCustomerAndProductType(Guid customerId, PolicyProductType productTypeToCheck);
}
