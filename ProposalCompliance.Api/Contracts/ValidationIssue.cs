namespace ProposalCompliance.Api.Contracts;

public sealed record ValidationIssue(string Field, string Message);
