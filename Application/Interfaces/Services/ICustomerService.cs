using Application.DTOs;

namespace Application.Interfaces.Services;

public interface ICustomerService
{
    Guid CreateCustomer(CreateCustomerRequest request);
}
