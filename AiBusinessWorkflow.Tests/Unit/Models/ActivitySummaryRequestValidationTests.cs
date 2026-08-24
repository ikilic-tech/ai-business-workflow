using System.ComponentModel.DataAnnotations;
using AiBusinessWorkflow.Api.Models;
using FluentAssertions;

namespace AiBusinessWorkflow.Tests.Unit.Models;

public class ActivitySummaryRequestValidationTests
{
    private static List<ValidationResult> ValidateModel(ActivitySummaryRequest request)
    {
        var context = new ValidationContext(request);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(request, context, results, validateAllProperties: true);
        return results;
    }

    private static ActivitySummaryRequest CreateValidRequest() => new()
    {
        Department = "Sales",
        Period = "Q1 2024",
        Activities = new List<ActivityEntry>
        {
            new()
            {
                EmployeeName = "Alice Johnson",
                ActivityType = "Cold Call",
                Date = "2024-01-08",
                Duration = "25 minutes",
                Description = "Outbound call to prospect company",
                Result = "Meeting scheduled"
            }
        }
    };

    [Fact]
    public void ValidRequest_ShouldPassValidation()
    {
        var request = CreateValidRequest();
        var results = ValidateModel(request);
        results.Should().BeEmpty();
    }

    [Fact]
    public void Department_WhenEmpty_ShouldFailValidation()
    {
        var request = CreateValidRequest();
        request.Department = string.Empty;
        var results = ValidateModel(request);
        results.Should().Contain(r => r.MemberNames.Contains("Department"));
    }

    [Fact]
    public void Department_WhenTooShort_ShouldFailValidation()
    {
        var request = CreateValidRequest();
        request.Department = "A";
        var results = ValidateModel(request);
        results.Should().Contain(r => r.MemberNames.Contains("Department"));
    }

    [Fact]
    public void Department_WhenTooLong_ShouldFailValidation()
    {
        var request = CreateValidRequest();
        request.Department = new string('A', 101);
        var results = ValidateModel(request);
        results.Should().Contain(r => r.MemberNames.Contains("Department"));
    }

    [Fact]
    public void Period_WhenEmpty_ShouldFailValidation()
    {
        var request = CreateValidRequest();
        request.Period = string.Empty;
        var results = ValidateModel(request);
        results.Should().Contain(r => r.MemberNames.Contains("Period"));
    }

    [Fact]
    public void Period_WhenTooShort_ShouldFailValidation()
    {
        var request = CreateValidRequest();
        request.Period = "Q1";
        var results = ValidateModel(request);
        results.Should().Contain(r => r.MemberNames.Contains("Period"));
    }

    [Fact]
    public void Period_AtMinLength_ShouldPassValidation()
    {
        var request = CreateValidRequest();
        request.Period = "Jan";
        var results = ValidateModel(request);
        results.Should().NotContain(r => r.MemberNames.Contains("Period"));
    }

    [Fact]
    public void Activities_WhenEmpty_ShouldFailValidation()
    {
        var request = CreateValidRequest();
        request.Activities = new List<ActivityEntry>();
        var results = ValidateModel(request);
        results.Should().Contain(r => r.MemberNames.Contains("Activities"));
    }

    [Fact]
    public void Activities_WhenExceedsMaxLength_ShouldFailValidation()
    {
        var request = CreateValidRequest();
        request.Activities = Enumerable.Range(0, 101).Select(_ => new ActivityEntry
        {
            EmployeeName = "Alice Johnson",
            ActivityType = "Cold Call",
            Date = "2024-01-08",
            Duration = "25 minutes",
            Description = "Outbound call to prospect company",
            Result = "Meeting scheduled"
        }).ToList();
        var results = ValidateModel(request);
        results.Should().Contain(r => r.MemberNames.Contains("Activities"));
    }

    [Fact]
    public void AllFieldsEmpty_ShouldFailMultipleValidations()
    {
        var request = new ActivitySummaryRequest
        {
            Department = string.Empty,
            Period = string.Empty,
            Activities = new List<ActivityEntry>()
        };
        var results = ValidateModel(request);
        results.Should().HaveCountGreaterOrEqualTo(3);
    }
}
