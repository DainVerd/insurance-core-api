using Application.DTOs;
using AwesomeAssertions;
using Domain.Constants;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace WebApi.IntegrationTests;

public class ClaimEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ClaimEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    #region CreateClaim tests
    [Fact]
    public async Task CreateClaim_Returns201_WhenValidRequest()
    {
        // Arrange
        var customerResponse = await _client.PostAsJsonAsync("/api/v1.0/customers", new { FullName = "John Doe" });
        var customerId = await customerResponse.Content.ReadFromJsonAsync<Guid>();

        var policyRequest = new CreatePolicyRequest
        {
            CustomerId = customerId,
            ProductType = PolicyProductType.Auto,
            StartDate = new DateOnly(2026, 07, 01),
            EndDate = new DateOnly(2026, 07, 27),
            Premium = 500,
        };
        var policyResponse = await _client.PostAsJsonAsync("/api/v1.0/policies", policyRequest);
        var policyId = await policyResponse.Content.ReadFromJsonAsync<Guid>();
        await _client.PostAsync($"/api/v1.0/policies/{policyId}/activate", null);
        var request = new CreateClaimRequest
        {
            PolicyId = policyId,
            IncidentDate = new DateTimeOffset(2026, 7, 9, 12, 12, 12, TimeSpan.Zero),
            AmountRequested = 333,
            DecisionReason = null
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1.0/claims", request);
        var responseData = await response.Content.ReadFromJsonAsync<Guid>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        responseData.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateClaim_Returns400_WhenAmountRequestedIsNegative()
    {
        // Arrange
        var customerResponse = await _client.PostAsJsonAsync("/api/v1.0/customers", new { FullName = "Jane Doe" });
        var customerId = await customerResponse.Content.ReadFromJsonAsync<Guid>();

        var policyRequest = new CreatePolicyRequest
        {
            CustomerId = customerId,
            ProductType = PolicyProductType.Auto,
            StartDate = new DateOnly(2026, 07, 01),
            EndDate = new DateOnly(2026, 07, 27),
            Premium = 500,
        };
        var policyResponse = await _client.PostAsJsonAsync("/api/v1.0/policies", policyRequest);
        var policyId = await policyResponse.Content.ReadFromJsonAsync<Guid>();
        await _client.PostAsync($"/api/v1.0/policies/{policyId}/activate", null);

        var request = new CreateClaimRequest
        {
            PolicyId = policyId,
            IncidentDate = new DateTimeOffset(2026, 7, 9, 12, 12, 12, TimeSpan.Zero),
            AmountRequested = -100
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1.0/claims", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateClaim_Returns404_WhenPolicyDoesNotExist()
    {
        var request = new CreateClaimRequest
        {
            PolicyId = Guid.NewGuid(),
            IncidentDate = new DateTimeOffset(2026, 7, 9, 12, 12, 12, TimeSpan.Zero),
            AmountRequested = 100
        };

        var response = await _client.PostAsJsonAsync("/api/v1.0/claims", request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateClaim_Returns409_WhenPolicyIsNotActive()
    {
        var customerResponse = await _client.PostAsJsonAsync("/api/v1.0/customers", new { FullName = "Jon Snow" });
        var customerId = await customerResponse.Content.ReadFromJsonAsync<Guid>();

        var policyRequest = new CreatePolicyRequest
        {
            CustomerId = customerId,
            ProductType = PolicyProductType.Auto,
            StartDate = new DateOnly(2026, 07, 01),
            EndDate = new DateOnly(2026, 07, 27),
            Premium = 500,
        };
        var policyResponse = await _client.PostAsJsonAsync("/api/v1.0/policies", policyRequest);
        var policyId = await policyResponse.Content.ReadFromJsonAsync<Guid>();

        var request = new CreateClaimRequest
        {
            PolicyId = policyId,
            IncidentDate = new DateTimeOffset(2026, 7, 9, 12, 12, 12, TimeSpan.Zero),
            AmountRequested = 100
        };

        var response = await _client.PostAsJsonAsync("/api/v1.0/claims", request);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
    #endregion
    #region GetClaim tests
    #endregion
    #region DecideClaim tests
    #endregion
}
