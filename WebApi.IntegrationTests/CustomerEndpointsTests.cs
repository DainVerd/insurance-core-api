using Application.DTOs;
using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace WebApi.IntegrationTests;

public class CustomerEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CustomerEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    #region CreateCustomer tests
    [Fact]
    public async Task CreateCustomer_Returns201_WhenValidRequest()
    {
        // Arrange
        var request = new { FullName = "Test Customer" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1.0/customers", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public async Task CreateCustomer_Returns400_WhenInvalidRequest(string? fullName)
    {
        // Arrange
        var request = new { FullName = fullName };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1.0/customers", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion
    #region GetCustomer tests
    [Fact]
    public async Task GetCustomer_Returns200_WhenFoundCustomer()
    {
        // Arrange
        var request = new { FullName = "Test Customer" };

        // Act
        var customersCreateResponse = await _client.PostAsJsonAsync("/api/v1.0/customers", request);
        var customerId = await customersCreateResponse.Content.ReadFromJsonAsync<Guid>();

        var response = await _client.GetAsync($"/api/v1.0/customers/{customerId}");
        var customerDto = await response.Content.ReadFromJsonAsync<CustomerDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        customerDto?.Id.Should().Be(customerId);
    }

    [Fact]
    public async Task GetCustomer_Returns404_WhenNotFoundCustomer()
    {

        // Arrange & Act
        var response = await _client.GetAsync($"/api/v1.0/customers/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetCustomer_Returns400_WhenInvalidCustomerIdProvided()
    {

        // Arrange & Act
        var response = await _client.GetAsync($"/api/v1.0/customers/{Guid.Empty}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    #endregion
}
