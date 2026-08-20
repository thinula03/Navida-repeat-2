using Microsoft.AspNetCore.Mvc;
using ProposalCompliance.Api.Contracts;
using ProposalCompliance.Api.Models;
using ProposalCompliance.Api.Services;

namespace ProposalCompliance.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class ProposalController(IComplianceAnalysisService complianceAnalysisService) : ControllerBase
{
    [HttpPost("analyse")]
    [ProducesResponseType(typeof(ProposalComplianceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProposalComplianceResponse), StatusCodes.Status400BadRequest)]
    public ActionResult<ProposalComplianceResponse> Analyse([FromBody] ProposalRequest request)
    {
        var alerts = complianceAnalysisService.Analyse(request);

        return alerts.Count == 0
            ? Ok(ProposalComplianceResponse.Approved())
            : Ok(ProposalComplianceResponse.ReviewRequired(alerts));
    }
}
