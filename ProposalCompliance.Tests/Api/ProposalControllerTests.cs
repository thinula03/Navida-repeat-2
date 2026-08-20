using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Testing;
using ProposalCompliance.Api.Contracts;
using ProposalCompliance.Api.Models;

namespace ProposalCompliance.Tests.Api;

public sealed class ProposalControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly JsonSerializerOptions ResponseJsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly HttpClient _client;

    public ProposalControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Analyse_WhenRequestIsValidAndLowRisk_ReturnsApprovedResponse()
    {
        var request = CreateRequest();

        using var response = await _client.PostAsJsonAsync("/api/proposal/analyse", request);
        var body = await response.Content.ReadFromJsonAsync<ProposalComplianceResponse>(ResponseJsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("Approved", body.ComplianceStatus);
        Assert.Empty(body.ValidationErrors);
        Assert.Empty(body.ComplianceAlerts);
    }

    [Fact]
    public async Task Analyse_WhenRequestFailsValidation_ReturnsStructuredValidationErrors()
    {
        var request = CreateRequest(quantity: 0, estimatedBudgetUsd: 0m);

        using var response = await _client.PostAsJsonAsync("/api/proposal/analyse", request);
        var body = await response.Content.ReadFromJsonAsync<ProposalComplianceResponse>(ResponseJsonOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("ValidationFailed", body.ComplianceStatus);
        Assert.Empty(body.ComplianceAlerts);
        Assert.Contains(body.ValidationErrors, error => error.Field == nameof(ProposalRequest.Quantity));
        Assert.Contains(body.ValidationErrors, error => error.Field == nameof(ProposalRequest.EstimatedBudgetUSD));
    }

    [Fact]
    public async Task Analyse_WhenComplianceRulesMatch_ReturnsReviewRequiredWithAlerts()
    {
        var request = CreateRequest(
            resourceRequested: "Advanced GPU Cluster",
            quantity: 4,
            ethicsRiskCountry: "Bermuda",
            estimatedBudgetUsd: 9000m);

        using var response = await _client.PostAsJsonAsync("/api/proposal/analyse", request);
        var body = await response.Content.ReadFromJsonAsync<ProposalComplianceResponse>(ResponseJsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("ReviewRequired", body.ComplianceStatus);
        Assert.Empty(body.ValidationErrors);
        Assert.Equal(3, body.ComplianceAlerts.Count);
    }

    private static ProposalRequest CreateRequest(
        string resourceRequested = "Survey Platform",
        int quantity = 1,
        string ethicsRiskCountry = "Sri Lanka",
        decimal estimatedBudgetUsd = 1500m)
        => new()
        {
            StudentName = "Navida Perera",
            ResourceRequested = resourceRequested,
            Quantity = quantity,
            EthicsRiskCountry = ethicsRiskCountry,
            EstimatedBudgetUSD = estimatedBudgetUsd,
            ResearchArea = "Digital Systems"
        };
}
