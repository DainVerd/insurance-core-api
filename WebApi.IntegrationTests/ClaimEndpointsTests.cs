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
    [Fact]
    public async Task GetClaim_Returns200AndCorrectData_WhenClaimExists()
    {
        // Arrange 
        var customerResponse = await _client.PostAsJsonAsync("/api/v1.0/customers", new { FullName = "Ned Stark" });
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

        var incidentDate = new DateTimeOffset(2026, 7, 9, 12, 12, 12, TimeSpan.Zero);
        var createRequest = new CreateClaimRequest
        {
            PolicyId = policyId,
            IncidentDate = incidentDate,
            AmountRequested = 250
        };
        var createResponse = await _client.PostAsJsonAsync("/api/v1.0/claims", createRequest);
        var claimId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        // Act
        var response = await _client.GetAsync($"/api/v1.0/claims/{claimId}");
        var claimDto = await response.Content.ReadFromJsonAsync<ClaimDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        claimDto.Should().NotBeNull();
        claimDto!.Id.Should().Be(claimId);
    }

    [Fact]
    public async Task GetClaim_Returns404_WhenClaimDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/v1.0/claims/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    #endregion
    #region DecideClaim tests
    [Fact]
    public async Task DecideClaim_Returns204_WhenApprovingValidClaim()
    {
        // Arrange
        var claimId = await CreateActiveClaimAsync();

        var decideRequest = new DecideClaimRequest
        {
            Status = ClaimStatus.Approved,
            DecisionReason = null
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/v1.0/claims/{claimId}/decide", decideRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/v1.0/claims/{claimId}");
        var claimDto = await getResponse.Content.ReadFromJsonAsync<ClaimDto>();
        claimDto!.Status.Should().Be(ClaimStatus.Approved);
    }

    [Fact]
    public async Task DecideClaim_Returns400_WhenRejectingWithoutDecisionReason()
    {
        // Arrange
        var claimId = await CreateActiveClaimAsync();

        var decideRequest = new DecideClaimRequest
        {
            Status = ClaimStatus.Rejected,
            DecisionReason = null
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/v1.0/claims/{claimId}/decide", decideRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DecideClaim_Returns404_WhenClaimDoesNotExist()
    {
        // Arrange
        var decideRequest = new DecideClaimRequest
        {
            Status = ClaimStatus.Rejected,
            DecisionReason = "Not enough evidence"
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/v1.0/claims/{Guid.NewGuid()}/decide", decideRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DecideClaim_Returns409_WhenClaimAlreadyDecided()
    {
        // Arrange
        var claimId = await CreateActiveClaimAsync();

        var firstDecide = new DecideClaimRequest { Status = ClaimStatus.Approved };
        await _client.PostAsJsonAsync($"/api/v1.0/claims/{claimId}/decide", firstDecide);

        var secondDecide = new DecideClaimRequest { Status = ClaimStatus.Rejected, DecisionReason = "Too late" };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/v1.0/claims/{claimId}/decide", secondDecide);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    private async Task<Guid> CreateActiveClaimAsync()
    {
        var customerResponse = await _client.PostAsJsonAsync("/api/v1.0/customers", new { FullName = "Sansa Stark" });
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

        var createRequest = new CreateClaimRequest
        {
            PolicyId = policyId,
            IncidentDate = new DateTimeOffset(2026, 7, 9, 12, 12, 12, TimeSpan.Zero),
            AmountRequested = 250
        };
        var createResponse = await _client.PostAsJsonAsync("/api/v1.0/claims", createRequest);
        return await createResponse.Content.ReadFromJsonAsync<Guid>();
    }
    #endregion
}
