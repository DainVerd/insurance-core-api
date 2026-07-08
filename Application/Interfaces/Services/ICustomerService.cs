using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces.Services;

public interface ICustomerService
{
    Guid CreateCustomer(CreateCustomerRequest request);

    Customer? GetCustomerById(Guid id);
}
