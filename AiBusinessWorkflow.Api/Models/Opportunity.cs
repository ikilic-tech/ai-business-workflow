using System.ComponentModel.DataAnnotations;

namespace AiBusinessWorkflow.Api.Models;

public class Opportunity
{
    public string OpportunityId { get; set; } = Guid.NewGuid().ToString();

    [Required(ErrorMessage = "AccountName is required.")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "AccountName must be between 3 and 200 characters.")]
    public string AccountName { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "DealValue must be greater than zero.")]
    public decimal DealValue { get; set; }

    [Required(ErrorMessage = "Stage is required.")]
    [StringLength(50, ErrorMessage = "Stage must be at most 50 characters.")]
    public string Stage { get; set; } = string.Empty;

    [Required(ErrorMessage = "ExpectedCloseDate is required.")]
    [StringLength(30, ErrorMessage = "ExpectedCloseDate must be at most 30 characters.")]
    public string ExpectedCloseDate { get; set; } = string.Empty;

    [Required(ErrorMessage = "CompetitorInfo is required.")]
    [StringLength(1000, MinimumLength = 5, ErrorMessage = "CompetitorInfo must be between 5 and 1000 characters.")]
    public string CompetitorInfo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Notes is required.")]
    [StringLength(2000, MinimumLength = 5, ErrorMessage = "Notes must be between 5 and 2000 characters.")]
    public string Notes { get; set; } = string.Empty;

    [Required(ErrorMessage = "Activities is required.")]
    [MinLength(1, ErrorMessage = "At least one activity is required.")]
    [MaxLength(100, ErrorMessage = "Maximum 100 activities allowed.")]
    public List<OpportunityActivity> Activities { get; set; } = new();
}

public class OpportunityActivity
{
    [Required(ErrorMessage = "Type is required.")]
    [StringLength(50, ErrorMessage = "Type must be at most 50 characters.")]
    public string Type { get; set; } = string.Empty;

    [Required(ErrorMessage = "Date is required.")]
    [StringLength(30, ErrorMessage = "Date must be at most 30 characters.")]
    public string Date { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(500, MinimumLength = 5, ErrorMessage = "Description must be between 5 and 500 characters.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "ContactPerson is required.")]
    [StringLength(100, ErrorMessage = "ContactPerson must be at most 100 characters.")]
    public string ContactPerson { get; set; } = string.Empty;
}
