using ProposalCompliance.Api.Contracts;
using ProposalCompliance.Api.Models;
using ProposalCompliance.Api.Services;

namespace ProposalCompliance.Tests.Compliance;

public sealed class ComplianceAnalysisServiceTests
{
    private readonly ComplianceAnalysisService _service = new();

    [Fact]
    public void Analyse_WhenProposalIsLowRisk_ReturnsNoAlerts()
    {
        var proposal = CreateProposal();

        var alerts = _service.Analyse(proposal);

        Assert.Empty(alerts);
    }

    [Fact]
    public void Analyse_WhenBudgetExceedsThreshold_ReturnsLowSeverityBudgetWarning()
    {
        var proposal = CreateProposal(estimatedBudgetUsd: 5000.01m);

        var alert = Assert.Single(_service.Analyse(proposal));
        Assert.Equal("BUDGET-001", alert.RuleCode);
        Assert.Equal(ComplianceSeverity.Low, alert.Severity);
    }

    [Fact]
    public void Analyse_WhenAdvancedGpuQuantityExceedsLimit_ReturnsHighSeverityResourceAlert()
    {
        var proposal = CreateProposal(resourceRequested: "Advanced GPU Cluster", quantity: 4);

        var alert = Assert.Single(_service.Analyse(proposal));
        Assert.Equal("RESOURCE-001", alert.RuleCode);
        Assert.Equal(ComplianceSeverity.High, alert.Severity);
    }

    [Fact]
    public void Analyse_WhenCountryIsRestricted_ReturnsHighSeverityEthicsAlert()
    {
        var proposal = CreateProposal(ethicsRiskCountry: "Bermuda");

        var alert = Assert.Single(_service.Analyse(proposal));
        Assert.Equal("ETHICS-001", alert.RuleCode);
        Assert.Equal(ComplianceSeverity.High, alert.Severity);
    }

    [Fact]
    public void Analyse_WhenMultipleRulesMatch_ReturnsEveryApplicableAlert()
    {
        var proposal = CreateProposal(
            resourceRequested: "Advanced GPU Cluster",
            quantity: 6,
            ethicsRiskCountry: "Bermuda",
            estimatedBudgetUsd: 12000m);

        var alerts = _service.Analyse(proposal);

        Assert.Equal(3, alerts.Count);
        Assert.Contains(alerts, alert => alert.RuleCode == "BUDGET-001");
        Assert.Contains(alerts, alert => alert.RuleCode == "RESOURCE-001");
        Assert.Contains(alerts, alert => alert.RuleCode == "ETHICS-001");
    }

    private static ProposalRequest CreateProposal(
        string resourceRequested = "Cloud NLP Toolkit",
        int quantity = 1,
        string ethicsRiskCountry = "Sri Lanka",
        decimal estimatedBudgetUsd = 2500m)
        => new()
        {
            StudentName = "Navida Perera",
            ResourceRequested = resourceRequested,
            Quantity = quantity,
            EthicsRiskCountry = ethicsRiskCountry,
            EstimatedBudgetUSD = estimatedBudgetUsd,
            ResearchArea = "Applied AI"
        };
}
