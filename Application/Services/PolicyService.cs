using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Constants;
using Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Application.Services;

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
            throw new ValidationException("Provided Customer id has default value!");

        if (request.Premium < 0)
            throw new ValidationException("Provided Premium is less than zero!");

        if (request.EndDate <= request.StartDate)
            throw new ValidationException("End Date must be bigger than StartDate");

        var customer = _customerRepository.GetById(request.CustomerId);
        if (customer is null)
            throw new NotFoundException($"Customer with id: {request.CustomerId} does not exist!");

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

    public PolicyDto GetById(Guid id)
    {
        if (id == Guid.Empty)
            throw new ValidationException("Provided id has default value!");

        var policy = _policyRepository.GetById(id)
        ?? throw new NotFoundException($"Policy with ID {id} was not found.");

        return new PolicyDto
        {
            Id = policy.Id,
            CustomerId = policy.CustomerId,
            EndDate = policy.EndDate,
            Premium = policy.Premium,
            StartDate = policy.StartDate,
            Status = policy.Status,
            ProductType = policy.ProductType
        };
    }

    public void ActivatePolicy(Guid id)
    {
        if (id == Guid.Empty)
            throw new ValidationException("Provided Customer id has default value!");

        var policyToActivate = _policyRepository.GetById(id)
        ?? throw new NotFoundException($"Policy with ID {id} was not found.");

        if (policyToActivate.Status != PolicyStatus.Draft)
            throw new ConflictException("Only Draft policies can be activated.");

        var customerActivePolicies = _policyRepository
           .GetActivePoliciesByCustomerAndProductType(policyToActivate.CustomerId, policyToActivate.ProductType);

        var hasOverlap = customerActivePolicies
         .Any(existing => existing.Id != policyToActivate.Id && (policyToActivate.StartDate <= existing.EndDate && existing.StartDate <= policyToActivate.EndDate));

        if (hasOverlap)
            throw new ConflictException($"Customer already has an active {policyToActivate.ProductType} policy with overlapping dates.");

        policyToActivate.Status = PolicyStatus.Active;

        _policyRepository.Update(policyToActivate);
    }
}
