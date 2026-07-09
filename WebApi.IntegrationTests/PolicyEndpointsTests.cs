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
    public async Task CreateCustomer_ValidRequest_Returns201()
    {
        // Arrange
        var request = new { FullName = "Test Customer" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1.0/customers", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
