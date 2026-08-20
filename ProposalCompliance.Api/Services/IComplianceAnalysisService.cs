using ProposalCompliance.Api.Contracts;
using ProposalCompliance.Api.Models;

namespace ProposalCompliance.Api.Services;

public interface IComplianceAnalysisService
{
    IReadOnlyCollection<ComplianceAlert> Analyse(ProposalRequest proposal);
}
