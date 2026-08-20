using ProposalCompliance.Api.Contracts;
using ProposalCompliance.Api.Models;

namespace ProposalCompliance.Api.Services;

public sealed class ComplianceAnalysisService : IComplianceAnalysisService
{
    private const decimal HighBudgetThresholdUsd = 5000m;
    private const string AdvancedGpuCluster = "Advanced GPU Cluster";
    private const string RestrictedEthicsCountry = "Bermuda";

    public IReadOnlyCollection<ComplianceAlert> Analyse(ProposalRequest proposal)
    {
        ArgumentNullException.ThrowIfNull(proposal);

        var alerts = new List<ComplianceAlert>();

        if (proposal.EstimatedBudgetUSD > HighBudgetThresholdUsd)
        {
            alerts.Add(new ComplianceAlert(
                RuleCode: "BUDGET-001",
                Severity: ComplianceSeverity.Low,
                Message: "High funding limit detected. Requires manual academic coordinator review."));
        }

        if (IsAdvancedGpuClusterRequest(proposal))
        {
            alerts.Add(new ComplianceAlert(
                RuleCode: "RESOURCE-001",
                Severity: ComplianceSeverity.High,
                Message: "Potential resource monopolization alert for Advanced GPU Cluster."));
        }

        if (IsRestrictedEthicsRegion(proposal.EthicsRiskCountry))
        {
            alerts.Add(new ComplianceAlert(
                RuleCode: "ETHICS-001",
                Severity: ComplianceSeverity.High,
                Message: "High severity alert: Field research proposed in a restricted ethics region."));
        }

        return alerts;
    }

    private static bool IsAdvancedGpuClusterRequest(ProposalRequest proposal)
        => proposal.Quantity > 3
           && string.Equals(
               proposal.ResourceRequested.Trim(),
               AdvancedGpuCluster,
               StringComparison.OrdinalIgnoreCase);

    private static bool IsRestrictedEthicsRegion(string country)
        => string.Equals(country.Trim(), RestrictedEthicsCountry, StringComparison.OrdinalIgnoreCase);
}
