using System.ComponentModel.DataAnnotations;

namespace ProposalCompliance.Api.Models;

public sealed class ProposalRequest
{
    [Required(ErrorMessage = "Student name is required.")]
    [StringLength(120, MinimumLength = 2, ErrorMessage = "Student name must be between 2 and 120 characters.")]
    public string StudentName { get; init; } = string.Empty;

    [Required(ErrorMessage = "Requested resource is required.")]
    [StringLength(160, MinimumLength = 2, ErrorMessage = "Requested resource must be between 2 and 160 characters.")]
    public string ResourceRequested { get; init; } = string.Empty;

    [Range(1, 100, ErrorMessage = "Quantity must be greater than 0 and no more than 100.")]
    public int Quantity { get; init; }

    [Required(ErrorMessage = "Ethics risk country is required.")]
    [StringLength(80, MinimumLength = 2, ErrorMessage = "Ethics risk country must be between 2 and 80 characters.")]
    public string EthicsRiskCountry { get; init; } = string.Empty;

    [Range(typeof(decimal), "0.01", "1000000", ErrorMessage = "Estimated budget must be greater than 0.")]
    public decimal EstimatedBudgetUSD { get; init; }

    [StringLength(120, ErrorMessage = "Research area cannot exceed 120 characters.")]
    public string? ResearchArea { get; init; }
}
