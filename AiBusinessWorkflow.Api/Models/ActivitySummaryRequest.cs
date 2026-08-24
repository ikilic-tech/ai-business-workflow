using System.ComponentModel.DataAnnotations;

namespace AiBusinessWorkflow.Api.Models;

public class ActivitySummaryRequest
{
    [Required(ErrorMessage = "Department is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Department must be between 2 and 100 characters.")]
    public string Department { get; set; } = string.Empty;

    [Required(ErrorMessage = "Period is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Period must be between 3 and 50 characters.")]
    public string Period { get; set; } = string.Empty;

    [Required(ErrorMessage = "Activities is required.")]
    [MinLength(1, ErrorMessage = "At least one activity is required.")]
    [MaxLength(100, ErrorMessage = "Maximum 100 activities allowed.")]
    public List<ActivityEntry> Activities { get; set; } = new();
}

public class ActivityEntry
{
    [Required(ErrorMessage = "EmployeeName is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "EmployeeName must be between 2 and 100 characters.")]
    public string EmployeeName { get; set; } = string.Empty;

    [Required(ErrorMessage = "ActivityType is required.")]
    [StringLength(50, ErrorMessage = "ActivityType must be at most 50 characters.")]
    public string ActivityType { get; set; } = string.Empty;

    [Required(ErrorMessage = "Date is required.")]
    [StringLength(30, ErrorMessage = "Date must be at most 30 characters.")]
    public string Date { get; set; } = string.Empty;

    [Required(ErrorMessage = "Duration is required.")]
    [StringLength(50, ErrorMessage = "Duration must be at most 50 characters.")]
    public string Duration { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(500, MinimumLength = 5, ErrorMessage = "Description must be between 5 and 500 characters.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Result is required.")]
    [StringLength(200, ErrorMessage = "Result must be at most 200 characters.")]
    public string Result { get; set; } = string.Empty;
}
