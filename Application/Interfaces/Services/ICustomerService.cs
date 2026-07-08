using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces.Services;

public interface ICustomerService
{
    Guid CreateCustomer(CreateCustomerRequest request);

    CustomerDto GetCustomerById(Guid id);
}
