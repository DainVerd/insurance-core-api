using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Constants;
using Domain.Entities;
using Infrastructure.Repositories;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Services;

public class ClaimService : IClaimService
{
    private readonly IClaimRepository _claimRepository;
    private readonly IPolicyRepository _policyRepository;
    public ClaimService(IClaimRepository claimRepository, IPolicyRepository policyRepository)
    {
        _claimRepository = claimRepository;
        _policyRepository = policyRepository;
    }
    public Guid CreateClaim(CreateClaimRequest claimToAdd)
    {
        if (claimToAdd is null)
            throw new ArgumentNullException("claim to Add is null!");


        if (claimToAdd.AmountRequested <= 0)
            throw new ValidationException("AmountRequested must be greater than zero.");

        var policy = _policyRepository.GetById(claimToAdd.PolicyId)
            ?? throw new NotFoundException("Policy with this id does not exist!");

        if (policy.Status != PolicyStatus.Active)
            throw new ConflictException("Claims can only be created for Active policies.");

        var incidentDateOnly = DateOnly.FromDateTime(claimToAdd.IncidentDate.DateTime);
        if (incidentDateOnly <= policy.StartDate || incidentDateOnly >= policy.EndDate)
            throw new ConflictException("IncidentDate must be within the policy period.");

        var newClaim = new Claim
        {
            Id = Guid.NewGuid(),
            PolicyId = claimToAdd.PolicyId,
            IncidentDate = claimToAdd.IncidentDate,
            AmountRequested = claimToAdd.AmountRequested,
            Status = ClaimStatus.New,
            DecisionReason = null,
        };

        _claimRepository.Add(newClaim);

        return newClaim.Id;
    }

    public ClaimDto GetById(Guid id)
    {
        if (id == Guid.Empty)
            throw new ValidationException("Provided Claim id is default value!");

        var claim = _claimRepository.GetById(id)
        ?? throw new NotFoundException($"Claim with ID {id} was not found.");

        return new ClaimDto
        {
            Id = claim.Id,
            PolicyId = claim.PolicyId,
            IncidentDate = claim.IncidentDate,
            AmountRequested = claim.AmountRequested,
            Status = claim.Status,
            DecisionReason = claim.DecisionReason
        };
    }
}
