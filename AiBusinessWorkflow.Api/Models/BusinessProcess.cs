using System.ComponentModel.DataAnnotations;

namespace AiBusinessWorkflow.Api.Models;

public class BusinessProcess
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 200 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 2000 characters.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "InputData is required.")]
    [StringLength(5000, MinimumLength = 5, ErrorMessage = "InputData must be between 5 and 5000 characters.")]
    public string InputData { get; set; } = string.Empty;

    [Required(ErrorMessage = "Goal is required.")]
    [StringLength(1000, MinimumLength = 5, ErrorMessage = "Goal must be between 5 and 1000 characters.")]
    public string Goal { get; set; } = string.Empty;
}
