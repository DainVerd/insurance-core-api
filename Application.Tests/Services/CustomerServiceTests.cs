using Application.Interfaces.Repositories;
using Application.Services;
using AwesomeAssertions;
using Moq;

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
    #endregion

    #region GetCustomerById tests
    #endregion
}
