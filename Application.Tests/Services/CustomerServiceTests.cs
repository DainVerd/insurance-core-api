using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Services;
using AwesomeAssertions;
using Domain.Entities;
using Moq;
using System.ComponentModel.DataAnnotations;

namespace Application.Tests.Services;

public class CustomerServiceTests
{
    private readonly Mock<ICustomerRepository> _customerRepoMock = new();
    private readonly CustomerService _sut;

    public CustomerServiceTests()
    {
        _sut = new CustomerService(_customerRepoMock.Object);
    }

    #region CreateCustomer tests
    [Fact]
    public void CreateCustomer_ThrowArgumentException_WhenArgumentEntityIsNull()
    {
        // Arrange and Act
        var act = () => _sut.CreateCustomer(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void CreateCustomer_ThrowValidationException_WhenFullNameIsEmptyStringOrNull(string? fullName)
    {
        // Arrange
        var request = new CreateCustomerRequest { FullName = fullName! };

        // Act
        var act = () => _sut.CreateCustomer(request);

        // Assert
        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void CreateCustomer_CallsCustomerRepositoryAndReturnsId_OnSuccess()
    {
        // Arrange
        var fullName = "Johny Jostar";
        var request = new CreateCustomerRequest { FullName = fullName };

        //Act
        var result = _sut.CreateCustomer(request);

        // Assert
        result.Should().NotBeEmpty();
        _customerRepoMock.Verify(r => r.Add(It.Is<Customer>(
            c => c.Id == result && c.FullName == fullName)), Times.Once);
    }
    #endregion

    #region GetCustomerById tests
    [Fact]
    public void GetCustomerById_ThrowValidationException_WhenArgumentIdIsDefaultValue()
    {
        // Arrange and Act
        var act = () => _sut.GetCustomerById(Guid.Empty);

        // Assert
        act.Should().Throw<ValidationException>();
        _customerRepoMock.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public void GetCustomerById_ThrowNotFoundException_WhenCustomerWasNotFound()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        _customerRepoMock.Setup(v => v.GetById(customerId))
            .Returns((Customer)null!);

        // Act
        var act = () => _sut.GetCustomerById(customerId);

        // Assert
        act.Should().Throw<NotFoundException>();
    }

    [Fact]
    public void GetCustomerById_CustomerData_WhenCustomerWasFound()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var customerFullName = "Jolyn Kujo";
        var customer = new Customer { Id = customerId, FullName = customerFullName };
        _customerRepoMock.Setup(v => v.GetById(customerId))
            .Returns(customer);

        // Act
        var result = _sut.GetCustomerById(customerId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(customerId);
        result.FullName.Should().Be(customerFullName);
        _customerRepoMock.Verify(r => r.GetById(customerId), Times.Once);
    }
    #endregion
}
