using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Constants;
using Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Services;

public class PolicyService : IPolicyService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IPolicyRepository _policyRepository;
    public PolicyService(ICustomerRepository customerRepository, IPolicyRepository policyRepository)
    {
        _customerRepository = customerRepository;
        _policyRepository = policyRepository;

    }
    public Guid CreatePolicy(CreatePolicyRequest request)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request));

        if (request.CustomerId == Guid.Empty)
            throw new ValidationException("Provided Customer id is default value!");

        if (request.Premium < 0)
            throw new ValidationException("Provided Premium is less than zero!");

        if (request.EndDate <= request.StartDate)
            throw new ValidationException("End Date must be bigger than StartDate");

        var customer = _customerRepository.GetById(request.CustomerId);
        if (customer is null)
            throw new NotFoundException("Customer with this id does not exist!");

        var policy = new Policy
        {
            Id = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            ProductType = request.ProductType,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Premium = request.Premium,
            Status = PolicyStatus.Draft
        };

        _policyRepository.Add(policy);

        return policy.Id;
    }

    public Policy? GePolicyById(Guid id)
    {
        if (id == Guid.Empty)
            throw new ValidationException("Provided Policy id is default value!");

        return _policyRepository.GetById(id);
    }
}
