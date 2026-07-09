using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Services;
using AwesomeAssertions;
using Castle.Core.Resource;
using Domain.Entities;
using Moq;
using System.ComponentModel.DataAnnotations;

namespace Application.Tests.Services;

public class PolicyServiceTests
{
    private readonly Mock<ICustomerRepository> _customerRepoMock = new();
    private readonly Mock<IPolicyRepository> _policyRepoMock = new();
    private readonly PolicyService _sut;

    public PolicyServiceTests()
    {
        _sut = new PolicyService(_customerRepoMock.Object, _policyRepoMock.Object);
    }

    #region CreatePolicy
    [Fact]
    public void CreatePolicy_ThrowArgumentNullException_WhenArgumentEntityIsNull()
    {
        // Arrange and Act
        var act = () => _sut.CreatePolicy(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CreatePolicy_ThrowValidationException_WhenProvidedArgumentCustomerIdIsDefaultValue()
    {
        // Arrange
        var request = new CreatePolicyRequest { CustomerId = Guid.Empty };
        // Act
        var act = () => _sut.CreatePolicy(request);

        // Assert
        act.Should().Throw<ValidationException>();
        _policyRepoMock.Verify(r => r.Add(It.IsAny<Policy>()), Times.Never);
        _customerRepoMock.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public void CreatePolicy_ThrowValidationException_WhenProvidedArgumentPremiumIsLessThanZero()
    {
        // Arrange
        var request = new CreatePolicyRequest { CustomerId = Guid.NewGuid(), Premium = -1 };
        // Act
        var act = () => _sut.CreatePolicy(request);

        // Assert
        act.Should().Throw<ValidationException>();
        _policyRepoMock.Verify(r => r.Add(It.IsAny<Policy>()), Times.Never);
        _customerRepoMock.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public void CreatePolicy_ThrowValidationException_WhenEndDateIsBiggerThanStartDate()
    {
        // Arrange
        var request = new CreatePolicyRequest
        {
            CustomerId = Guid.NewGuid(),
            Premium = 666,
            StartDate = new DateOnly(2026, 07, 31),
            EndDate = new DateOnly(2026, 01, 01)
        };
        // Act
        var act = () => _sut.CreatePolicy(request);

        // Assert
        act.Should().Throw<ValidationException>();
        _policyRepoMock.Verify(r => r.Add(It.IsAny<Policy>()), Times.Never);
        _customerRepoMock.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public void CreatePolicy_NotFoundException_WhenCustomerWasNotFound()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var request = new CreatePolicyRequest
        {
            CustomerId = customerId,
            Premium = 666,
            StartDate = new DateOnly(2026, 07, 01),
            EndDate = new DateOnly(2026, 07, 31)
        };
        _customerRepoMock.Setup(v => v.GetById(customerId))
            .Returns((Customer)null!);

        // Act
        var act = () => _sut.CreatePolicy(request);

        // Assert
        act.Should().Throw<NotFoundException>();
        _policyRepoMock.Verify(r => r.Add(It.IsAny<Policy>()), Times.Never);
        _customerRepoMock.Verify(r => r.GetById(customerId), Times.Once);
    }

    [Fact]
    public void CreatePolicy_CreateNewPolicy_OnSuccess()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var customer = new Customer
        {
            Id = customerId,
            FullName = "Test"
        };
        var request = new CreatePolicyRequest
        {
            CustomerId = customerId,
            Premium = 666,
            StartDate = new DateOnly(2026, 07, 01),
            EndDate = new DateOnly(2026, 07, 31)
        };
        _customerRepoMock.Setup(v => v.GetById(customerId))
            .Returns(customer);

        // Act
        var result = _sut.CreatePolicy(request);

        // Assert
        result.Should().NotBeEmpty();
        _policyRepoMock.Verify(r => r.Add(It.IsAny<Policy>()), Times.Once);
        _customerRepoMock.Verify(r => r.GetById(customerId), Times.Once);
    }

    #endregion
    #region GetById
    [Fact]
    public void GetById_ThrowValidationException_WhenIdIsDefaultValueOfTheType()
    {
        // Arrange and Act
        var act = () => _sut.GetById(Guid.Empty);

        // Assert
        act.Should().Throw<ValidationException>();
        _policyRepoMock.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public void GetById_ThrowNotFoundException_WhenPolicyWasNotFound()
    {
        // Arrange
        var policyId = Guid.NewGuid();
        _policyRepoMock.Setup(v => v.GetById(policyId))
            .Returns((Policy)null!);

        // Act
        var act = () => _sut.GetById(policyId);

        // Assert
        act.Should().Throw<NotFoundException>();
        _policyRepoMock.Verify(r => r.GetById(policyId), Times.Once);
    }

    [Fact]
    public void GetById_ReturnPolicy_WhenPolicyWasFound()
    {
        // Arrange
        var policyId = Guid.NewGuid();
        var policyToReturn = new Policy
        {
            Id = policyId,
        };
        _policyRepoMock.Setup(v => v.GetById(policyId))
            .Returns(policyToReturn);

        // Act
        var result = _sut.GetById(policyId);

        // Assert
        result.Id.Should().Be(policyToReturn.Id);
        _policyRepoMock.Verify(r => r.GetById(policyId), Times.Once);
    }

    #endregion

    #region ActivatePolicy
    #endregion
}
