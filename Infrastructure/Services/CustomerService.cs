using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }
    public Guid CreateCustomer(CreateCustomerRequest request)
    {
        if (string.IsNullOrEmpty(request.FullName))
            throw new ValidationException("Full name is required.");

        var customer = new Customer { Id = Guid.NewGuid(), FullName = request.FullName };
        _customerRepository.Add(customer);

        return customer.Id;
    }
}
