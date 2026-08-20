namespace ProposalCompliance.Api.Contracts;

public sealed record ComplianceAlert(
    string RuleCode,
    ComplianceSeverity Severity,
    string Message);
