using AwesomeAssertions;
using Domain.Constants;
using Domain.Entities;
using Infrastructure.Repositories;

namespace Infrastructure.Tests.Repositories;

public class PolicyRepositoryTests
{
    private readonly PolicyRepository _sut = new();

    [Fact]
    public void GetActivePoliciesByCustomerAndProductType_ReturnsOnlyMatchingActivePolicies()
    {
        var customerId = Guid.NewGuid();

        var matching = new Policy
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            ProductType = PolicyProductType.Auto,
            Status = PolicyStatus.Active,
            StartDate = new DateOnly(2026, 1, 1),
            EndDate = new DateOnly(2026, 1, 31)
        };

        var wrongStatus = new Policy
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            ProductType = PolicyProductType.Auto,
            Status = PolicyStatus.Draft,
            StartDate = new DateOnly(2026, 2, 1),
            EndDate = new DateOnly(2026, 2, 28)
        };

        var wrongProductType = new Policy
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            ProductType = PolicyProductType.Property,
            Status = PolicyStatus.Active,
            StartDate = new DateOnly(2026, 1, 1),
            EndDate = new DateOnly(2026, 1, 31)
        };

        var wrongCustomer = new Policy
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            ProductType = PolicyProductType.Auto,
            Status = PolicyStatus.Active,
            StartDate = new DateOnly(2026, 1, 1),
            EndDate = new DateOnly(2026, 1, 31)
        };

        _sut.Add(matching);
        _sut.Add(wrongStatus);
        _sut.Add(wrongProductType);
        _sut.Add(wrongCustomer);

        var result = _sut.GetActivePoliciesByCustomerAndProductType(customerId, PolicyProductType.Auto);

        result.Should().ContainSingle()
            .Which.Id.Should().Be(matching.Id);
    }

    [Fact]
    public void GetActivePoliciesByCustomerAndProductType_NoMatches_ReturnsEmpty()
    {
        var result = _sut.GetActivePoliciesByCustomerAndProductType(Guid.NewGuid(), PolicyProductType.Travel);

        result.Should().BeEmpty();
    }
}
