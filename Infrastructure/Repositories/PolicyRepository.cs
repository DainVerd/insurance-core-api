using Application.Interfaces.Repositories;
using Domain.Constants;
using Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Repositories;

public class PolicyRepository : BaseInMemoryRepository<Policy>, IPolicyRepository
{
    public IEnumerable<Policy> GetActivePoliciesByCustomerAndProductType(Guid customerId, PolicyProductType productTypeToCheck)
    {
        if (customerId == Guid.Empty)
            throw new ValidationException("Provided customer id has default value or null!");

        return [.. Storage.Values
            .Where(p => p.CustomerId == customerId
                     && p.ProductType == productTypeToCheck
                     && p.Status == PolicyStatus.Active)];
    }
}
