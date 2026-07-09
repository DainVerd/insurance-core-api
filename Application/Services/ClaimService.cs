using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Constants;
using Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Application.Services;

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
            throw new ArgumentNullException("Claim to Add is null!");

        if (claimToAdd.AmountRequested <= 0)
            throw new ValidationException("AmountRequested must be greater than zero.");

        var policy = _policyRepository.GetById(claimToAdd.PolicyId)
            ?? throw new NotFoundException($"Policy with this id: {claimToAdd.PolicyId} does not exist!");

        if (policy.Status != PolicyStatus.Active)
            throw new ConflictException("Claims can only be created for Active policies.");

        var incidentDateOnly = DateOnly.FromDateTime(claimToAdd.IncidentDate.DateTime);
        if (incidentDateOnly < policy.StartDate || incidentDateOnly > policy.EndDate)
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
            throw new ValidationException("Provided Claim id has default value!");

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

    public void Decide(Guid claimId, DecideClaimRequest request)
    {
        if (claimId == Guid.Empty)
            throw new ValidationException("Provided Claim id has default value.");

        if (request is null)
            throw new ArgumentNullException(nameof(request));

        var allowedStatuses = new List<ClaimStatus> { ClaimStatus.Approved, ClaimStatus.Rejected };

        if (!allowedStatuses.Contains(request.Status))
            throw new ValidationException($"Provided status: {request.Status} can not be used.");

        if (request.Status == ClaimStatus.Rejected && string.IsNullOrWhiteSpace(request.DecisionReason))
            throw new ValidationException("DecisionReason is required when rejecting a claim.");

        if (request.Status == ClaimStatus.Approved && !string.IsNullOrWhiteSpace(request.DecisionReason))
            throw new ValidationException("DecisionReason must not be provided when approving a claim.");

        var claimToDecide = _claimRepository.GetById(claimId)
        ?? throw new NotFoundException($"Claim with ID {claimId} was not found.");

        if (claimToDecide.Status != ClaimStatus.New)
            throw new ConflictException("Claim to decide does not have status New.");

        claimToDecide.Status = request.Status;
        claimToDecide.DecisionReason = request.DecisionReason;

        _claimRepository.Update(claimToDecide);
    }
}
