using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Services;
using AwesomeAssertions;
using Domain.Constants;
using Domain.Entities;
using Moq;
using System.ComponentModel.DataAnnotations;

namespace Application.Tests.Services;

public class ClaimServiceTests
{
    private readonly Mock<IClaimRepository> _claimRepoMock = new();
    private readonly Mock<IPolicyRepository> _policyRepoMock = new();
    private readonly ClaimService _sut;

    public ClaimServiceTests()
    {
        _sut = new ClaimService(_claimRepoMock.Object, _policyRepoMock.Object);
    }

    #region CreateClaim tests
    [Fact]
    public void CreateClaim_ThrowArgumentException_WhenArgumentRequestClaimIsNullEntity()
    {
        // Arrange and Act
        var act = () => _sut.CreateClaim(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
        _claimRepoMock.Verify(r => r.Add(It.IsAny<Claim>()), Times.Never);
        _policyRepoMock.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public void CreateClaim_ThrowValidationException_WhenAmountRequestedIsNegativeNumber()
    {
        // Arrange 
        var claimToCreate = new CreateClaimRequest { AmountRequested = -1 };

        //Act
        var act = () => _sut.CreateClaim(claimToCreate);

        // Assert
        act.Should().Throw<ValidationException>();
        _claimRepoMock.Verify(r => r.Add(It.IsAny<Claim>()), Times.Never);
        _policyRepoMock.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public void CreateClaim_ThrowNotFoundException_WhenPolicyWasNotFound()
    {
        // Arrange 
        var claimToCreate = new CreateClaimRequest { PolicyId = Guid.NewGuid(), AmountRequested = 66 };
        _policyRepoMock.Setup(v => v.GetById(It.IsAny<Guid>()))
            .Returns((Policy)null!);

        //Act
        var act = () => _sut.CreateClaim(claimToCreate);

        // Assert
        act.Should().Throw<NotFoundException>();
        _claimRepoMock.Verify(r => r.Add(It.IsAny<Claim>()), Times.Never);
        _policyRepoMock.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public void CreateClaim_ThrowConflictException_WhenPolicyStatusIsNotActive()
    {
        // Arrange 
        var policyId = Guid.NewGuid();
        var claimToCreate = new CreateClaimRequest { PolicyId = policyId, AmountRequested = 66 };
        var policy = new Policy { Id = policyId, Status = Domain.Constants.PolicyStatus.Draft };
        _policyRepoMock.Setup(v => v.GetById(policyId))
            .Returns(policy);

        //Act
        var act = () => _sut.CreateClaim(claimToCreate);

        // Assert
        act.Should().Throw<ConflictException>();
        _claimRepoMock.Verify(r => r.Add(It.IsAny<Claim>()), Times.Never);
        _policyRepoMock.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public void CreateClaim_ThrowConflictException_WhenClaimIncidentDateIsAfterInRangeOfPolicy()
    {
        // Arrange 
        var policyId = Guid.NewGuid();
        var incidentDate = new DateTimeOffset(2026, 8, 1, 12, 12, 12, TimeSpan.Zero);
        var claimToCreate = new CreateClaimRequest { PolicyId = policyId, AmountRequested = 66, IncidentDate = incidentDate };
        var policy = new Policy
        {
            Id = policyId,
            Status = Domain.Constants.PolicyStatus.Active,
            StartDate = new DateOnly(2026, 07, 01),
            EndDate = new DateOnly(2026, 07, 31),
        };
        _policyRepoMock.Setup(v => v.GetById(policyId))
            .Returns(policy);

        //Act
        var act = () => _sut.CreateClaim(claimToCreate);

        // Assert
        act.Should().Throw<ConflictException>();
        _claimRepoMock.Verify(r => r.Add(It.IsAny<Claim>()), Times.Never);
        _policyRepoMock.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public void CreateClaim_ThrowConflictException_WhenClaimIncidentDateIsBeforeInRangeOfPolicy()
    {
        // Arrange 
        var policyId = Guid.NewGuid();
        var incidentDate = new DateTimeOffset(2025, 8, 1, 12, 12, 12, TimeSpan.Zero);
        var claimToCreate = new CreateClaimRequest { PolicyId = policyId, AmountRequested = 66, IncidentDate = incidentDate };
        var policy = new Policy
        {
            Id = policyId,
            Status = Domain.Constants.PolicyStatus.Active,
            StartDate = new DateOnly(2026, 07, 01),
            EndDate = new DateOnly(2026, 07, 31),
        };
        _policyRepoMock.Setup(v => v.GetById(policyId))
            .Returns(policy);

        //Act
        var act = () => _sut.CreateClaim(claimToCreate);

        // Assert
        act.Should().Throw<ConflictException>();
        _claimRepoMock.Verify(r => r.Add(It.IsAny<Claim>()), Times.Never);
        _policyRepoMock.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public void CreateClaim_ReturnId_OnSuccesfullCreationOfClaim()
    {
        // Arrange 
        var policyId = Guid.NewGuid();
        var incidentDate = new DateTimeOffset(2026, 7, 16, 12, 12, 12, TimeSpan.Zero);
        var claimToCreate = new CreateClaimRequest { PolicyId = policyId, AmountRequested = 66, IncidentDate = incidentDate };
        var policy = new Policy
        {
            Id = policyId,
            Status = Domain.Constants.PolicyStatus.Active,
            StartDate = new DateOnly(2026, 07, 01),
            EndDate = new DateOnly(2026, 07, 31),
        };
        _policyRepoMock.Setup(v => v.GetById(policyId))
            .Returns(policy);

        //Act
        var result = _sut.CreateClaim(claimToCreate);

        // Assert
        result.Should().NotBeEmpty();
        _claimRepoMock.Verify(r => r.Add(It.IsAny<Claim>()), Times.Once);
        _policyRepoMock.Verify(r => r.GetById(policyId), Times.Once);
    }
    #endregion
    #region GetById tests
    [Fact]
    public void GetById_ThrowValidationException_WhenProvidedIdHasDefaultValue()
    {
        // Arrange and Act
        var act = () => _sut.GetById(Guid.Empty);

        // Assert
        act.Should().Throw<ValidationException>();
        _claimRepoMock.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public void GetById_ThrowNotFoundException_WhenClaimWasNotFound()
    {
        // Arrange
        var claimId = Guid.NewGuid();
        _claimRepoMock.Setup(v => v.GetById(claimId))
            .Returns((Claim)null!);
        // Act
        var act = () => _sut.GetById(claimId);

        // Assert
        act.Should().Throw<NotFoundException>();
        _claimRepoMock.Verify(r => r.GetById(claimId), Times.Once);
    }

    [Fact]
    public void GetById_ReturnsEntity_OnSuccess()
    {
        // Arrange
        var claimId = Guid.NewGuid();
        var claim = new Claim
        {
            Id = claimId,
            PolicyId = Guid.NewGuid(),
            IncidentDate = DateTime.UtcNow,
            AmountRequested = 0,
            Status = Domain.Constants.ClaimStatus.New,
            DecisionReason = null
        };
        _claimRepoMock.Setup(v => v.GetById(claimId))
            .Returns(claim);

        // Act
        var result = _sut.GetById(claimId);

        // Assert
        result.Id.Should().Be(claimId);
        _claimRepoMock.Verify(r => r.GetById(claimId), Times.Once);
    }
    #endregion
    #region Decide tests
    [Fact]
    public void Decide_ThrowValidationException_WhenProvidedIdHasDefaultValue()
    {
        // Arrange and Act
        var act = () => _sut.Decide(Guid.Empty, null!);

        // Assert
        act.Should().Throw<ValidationException>();
        _claimRepoMock.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
        _claimRepoMock.Verify(r => r.Update(It.IsAny<Claim>()), Times.Never);
    }

    [Fact]
    public void Decide_ThrowArgumentNullException_WhenProvidedRequestEntityIsNull()
    {
        // Arrange and Act
        var act = () => _sut.Decide(Guid.NewGuid(), null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
        _claimRepoMock.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
        _claimRepoMock.Verify(r => r.Update(It.IsAny<Claim>()), Times.Never);
    }

    [Fact]
    public void Decide_ThrowValidationException_WhenProvidedWithNotAllowedStatus()
    {
        // Arrange
        var claimId = Guid.NewGuid();
        var request = new DecideClaimRequest
        {
            Status = ClaimStatus.New,
        };
        // Act
        var act = () => _sut.Decide(claimId, request);

        // Assert
        act.Should().Throw<ValidationException>();
        _claimRepoMock.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
        _claimRepoMock.Verify(r => r.Update(It.IsAny<Claim>()), Times.Never);
    }

    [Fact]
    public void Decide_ThrowValidationException_WhenStatusIsNotRejectedButHasDecisionReason()
    {
        // Arrange
        var claimId = Guid.NewGuid();
        var request = new DecideClaimRequest
        {
            Status = ClaimStatus.Approved,
            DecisionReason = "Very important msg"
        };
        // Act
        var act = () => _sut.Decide(claimId, request);

        // Assert
        act.Should().Throw<ValidationException>();
        _claimRepoMock.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
        _claimRepoMock.Verify(r => r.Update(It.IsAny<Claim>()), Times.Never);
    }

    [Fact]
    public void Decide_ThrowNotFoundException_WhenClaimWasNotFound()
    {
        // Arrange
        var claimId = Guid.NewGuid();
        var request = new DecideClaimRequest
        {
            Status = ClaimStatus.Rejected,
            DecisionReason = "Very important msg"
        };
        _claimRepoMock.Setup(v => v.GetById(claimId))
            .Returns((Claim)null!);

        // Act
        var act = () => _sut.Decide(claimId, request);

        // Assert
        act.Should().Throw<NotFoundException>();
        _claimRepoMock.Verify(r => r.GetById(claimId), Times.Once);
        _claimRepoMock.Verify(r => r.Update(It.IsAny<Claim>()), Times.Never);
    }

    [Fact]
    public void Decide_ThrowConflictException_WhenClaimStatusIsNotNew()
    {
        // Arrange
        var claimId = Guid.NewGuid();
        var request = new DecideClaimRequest
        {
            Status = ClaimStatus.Rejected,
            DecisionReason = "Very important msg"
        };
        var claimToDecide = new Claim
        {
            Id = claimId,
            Status = ClaimStatus.Approved,
        };
        _claimRepoMock.Setup(v => v.GetById(claimId))
            .Returns(claimToDecide);

        // Act
        var act = () => _sut.Decide(claimId, request);

        // Assert
        act.Should().Throw<ConflictException>();
        _claimRepoMock.Verify(r => r.GetById(claimId), Times.Once);
        _claimRepoMock.Verify(r => r.Update(It.IsAny<Claim>()), Times.Never);
    }

    [Fact]
    public void Decide_UpdateClaim_OnSuccess()
    {
        // Arrange
        var claimId = Guid.NewGuid();
        var request = new DecideClaimRequest
        {
            Status = ClaimStatus.Rejected,
            DecisionReason = "Very important msg"
        };
        var claimToDecide = new Claim
        {
            Id = claimId,
            Status = ClaimStatus.New,
        };
        _claimRepoMock.Setup(v => v.GetById(claimId))
            .Returns(claimToDecide);

        // Act
        _sut.Decide(claimId, request);

        // Assert
        claimToDecide.Status.Should().Be(request.Status);
        claimToDecide.DecisionReason.Should().Be(request.DecisionReason);
        _claimRepoMock.Verify(r => r.GetById(claimId), Times.Once);
        _claimRepoMock.Verify(r => r.Update(claimToDecide), Times.Once);
    }

    [Fact]
    public void Decide_ThrowValidationException_WhenRejectingWithoutDecisionReason()
    {
        // Arrange
        var claimId = Guid.NewGuid();
        var request = new DecideClaimRequest
        {
            Status = ClaimStatus.Rejected,
            DecisionReason = null
        };
        var claimToDecide = new Claim
        {
            Id = claimId,
            Status = ClaimStatus.New,
        };
        _claimRepoMock.Setup(v => v.GetById(claimId))
            .Returns(claimToDecide);

        // Act
        var act = () => _sut.Decide(claimId, request);

        // Assert
        act.Should().Throw<ValidationException>();
        _claimRepoMock.Verify(r => r.Update(It.IsAny<Claim>()), Times.Never);
    }
    #endregion
}
