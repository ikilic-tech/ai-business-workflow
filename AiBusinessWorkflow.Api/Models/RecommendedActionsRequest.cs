using System.ComponentModel.DataAnnotations;

namespace AiBusinessWorkflow.Api.Models;

public class RecommendedActionsRequest
{
    [Required(ErrorMessage = "BusinessArea is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "BusinessArea must be between 2 and 100 characters.")]
    public string BusinessArea { get; set; } = string.Empty;

    [Required(ErrorMessage = "CurrentChallenges is required.")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "CurrentChallenges must be between 10 and 2000 characters.")]
    public string CurrentChallenges { get; set; } = string.Empty;

    [Required(ErrorMessage = "AvailableResources is required.")]
    [StringLength(1000, MinimumLength = 5, ErrorMessage = "AvailableResources must be between 5 and 1000 characters.")]
    public string AvailableResources { get; set; } = string.Empty;

    [Required(ErrorMessage = "Goals is required.")]
    [StringLength(1000, MinimumLength = 5, ErrorMessage = "Goals must be between 5 and 1000 characters.")]
    public string Goals { get; set; } = string.Empty;

    [Required(ErrorMessage = "RecentMetrics is required.")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "RecentMetrics must be between 10 and 2000 characters.")]
    public string RecentMetrics { get; set; } = string.Empty;
}
