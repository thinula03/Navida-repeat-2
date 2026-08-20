namespace ProposalCompliance.Api.Contracts;

public sealed record ProposalComplianceResponse(
    string ComplianceStatus,
    IReadOnlyCollection<ValidationIssue> ValidationErrors,
    IReadOnlyCollection<ComplianceAlert> ComplianceAlerts)
{
    public static ProposalComplianceResponse Approved()
        => new("Approved", Array.Empty<ValidationIssue>(), Array.Empty<ComplianceAlert>());

    public static ProposalComplianceResponse ReviewRequired(IReadOnlyCollection<ComplianceAlert> alerts)
        => new("ReviewRequired", Array.Empty<ValidationIssue>(), alerts);

    public static ProposalComplianceResponse ValidationFailed(IReadOnlyCollection<ValidationIssue> validationErrors)
        => new("ValidationFailed", validationErrors, Array.Empty<ComplianceAlert>());
}
