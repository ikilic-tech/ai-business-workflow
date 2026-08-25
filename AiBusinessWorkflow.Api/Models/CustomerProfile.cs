using System.ComponentModel.DataAnnotations;

namespace AiBusinessWorkflow.Api.Models;

public class CustomerProfile
{
    public string CustomerId { get; set; } = Guid.NewGuid().ToString();

    [Required(ErrorMessage = "CompanyName is required.")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "CompanyName must be between 3 and 200 characters.")]
    public string CompanyName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Industry is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Industry must be between 2 and 100 characters.")]
    public string Industry { get; set; } = string.Empty;

    public int EmployeeCount { get; set; }

    public decimal AnnualRevenue { get; set; }

    [Required(ErrorMessage = "ContactName is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "ContactName must be between 2 and 100 characters.")]
    public string ContactName { get; set; } = string.Empty;

    [Required(ErrorMessage = "ContactEmail is required.")]
    [EmailAddress(ErrorMessage = "ContactEmail must be a valid email address.")]
    [StringLength(200, ErrorMessage = "ContactEmail must be at most 200 characters.")]
    public string ContactEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "AccountAge is required.")]
    [StringLength(50, ErrorMessage = "AccountAge must be at most 50 characters.")]
    public string AccountAge { get; set; } = string.Empty;

    [Required(ErrorMessage = "PaymentHistory is required.")]
    [StringLength(500, MinimumLength = 5, ErrorMessage = "PaymentHistory must be between 5 and 500 characters.")]
    public string PaymentHistory { get; set; } = string.Empty;

    [Required(ErrorMessage = "Activities is required.")]
    [MinLength(1, ErrorMessage = "At least one activity is required.")]
    [MaxLength(100, ErrorMessage = "Maximum 100 activities allowed.")]
    public List<CustomerActivity> Activities { get; set; } = new();
}

public class CustomerActivity
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

    [Required(ErrorMessage = "Outcome is required.")]
    [StringLength(200, ErrorMessage = "Outcome must be at most 200 characters.")]
    public string Outcome { get; set; } = string.Empty;
}
