using Application.DTOs;
using Application.Exceptions;
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
        if (request is null)
            throw new ArgumentNullException("provided argument is null");

        if (string.IsNullOrEmpty(request.FullName))
            throw new ValidationException("Full name is required.");

        var customer = new Customer { Id = Guid.NewGuid(), FullName = request.FullName };
        _customerRepository.Add(customer);

        return customer.Id;
    }

    public CustomerDto GetCustomerById(Guid id)
    {
        if (id == Guid.Empty)
            throw new ValidationException("Id can not be default value or null");

        var customer = _customerRepository.GetById(id)
       ?? throw new NotFoundException($"Customer with ID {id} was not found.");

        return new CustomerDto { Id = customer.Id, FullName = customer.FullName };
    }
}
