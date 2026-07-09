using Application.DTOs;
using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace WebApi.IntegrationTests;

public class PolicyEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public PolicyEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ActivatePolicy_Returns409_WhenAlreadyActivePolicy()
    {
        // Arrange 
        // creating customer
        var customerResponse = await _client.PostAsJsonAsync("/api/v1.0/customers", new { FullName = "John Doe" });
        var customerId = await customerResponse.Content.ReadFromJsonAsync<Guid>();

        var policyRequest = new CreatePolicyRequest
        {
            CustomerId = customerId,
            ProductType = Domain.Constants.PolicyProductType.Auto,
            StartDate = new DateOnly(2026,01,01),
            EndDate = new DateOnly(2026,01,31),
            Premium = 500
        };
        var policyResponse = await _client.PostAsJsonAsync("/api/v1.0/policies", policyRequest);
        var policyId = await policyResponse.Content.ReadFromJsonAsync<Guid>();

        // Act 
        var firstActivate = await _client.PostAsync($"/api/v1.0/policies/{policyId}/activate", null);

        var secondActivate = await _client.PostAsync($"/api/v1.0/policies/{policyId}/activate", null);

        // Assert
        firstActivate.StatusCode.Should().Be(HttpStatusCode.NoContent);
        secondActivate.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
    [Fact]
    public async Task ActivatePolicy_OverlappingActivePolicySameCustomerAndProductType_Returns409()
    {
        // Arrange — customer
        var customerResponse = await _client.PostAsJsonAsync("/api/v1.0/customers", new { FullName = "John Doe" });
        var customerId = await customerResponse.Content.ReadFromJsonAsync<Guid>();

        var firstPolicyRequest = new CreatePolicyRequest
        {
            CustomerId = customerId,
            ProductType = Domain.Constants.PolicyProductType.Auto,
            StartDate = new DateOnly(2026, 01, 01),
            EndDate = new DateOnly(2026, 01, 31),
            Premium = 500
        };
        var firstPolicyResponse = await _client.PostAsJsonAsync("/api/v1.0/policies", firstPolicyRequest);
        var firstPolicyId = await firstPolicyResponse.Content.ReadFromJsonAsync<Guid>();
        await _client.PostAsync($"/api/v1.0/policies/{firstPolicyId}/activate", null);

        // second policy with overlaping date
        var secondPolicyRequest = new
        {
            CustomerId = customerId,
            ProductType = Domain.Constants.PolicyProductType.Auto,
            StartDate = new DateOnly(2026, 01, 15),
            EndDate = new DateOnly(2026, 04, 27),
            Premium = 300
        };
        var secondPolicyResponse = await _client.PostAsJsonAsync("/api/v1.0/policies", secondPolicyRequest);
        var secondPolicyId = await secondPolicyResponse.Content.ReadFromJsonAsync<Guid>();

        // Act
        var activateSecond = await _client.PostAsync($"/api/v1.0/policies/{secondPolicyId}/activate", null);

        // Assert
        activateSecond.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}
