using System.ComponentModel.DataAnnotations;
using AiBusinessWorkflow.Api.Models;
using FluentAssertions;

namespace AiBusinessWorkflow.Tests.Unit.Models;

public class RecommendedActionsRequestValidationTests
{
    private static List<ValidationResult> ValidateModel(RecommendedActionsRequest request)
    {
        var context = new ValidationContext(request);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(request, context, results, validateAllProperties: true);
        return results;
    }

    private static RecommendedActionsRequest CreateValidRequest() => new()
    {
        BusinessArea = "Sales Operations",
        CurrentChallenges = "Sales cycle has lengthened significantly over the past quarter",
        AvailableResources = "12 sales reps, CRM platform, training budget",
        Goals = "Reduce sales cycle, improve win rate",
        RecentMetrics = "Q4 2023: Revenue $2.1M against target of $2.5M"
    };

    [Fact]
    public void ValidRequest_ShouldPassValidation()
    {
        var request = CreateValidRequest();
        var results = ValidateModel(request);
        results.Should().BeEmpty();
    }

    [Fact]
    public void BusinessArea_WhenEmpty_ShouldFailValidation()
    {
        var request = CreateValidRequest();
        request.BusinessArea = string.Empty;
        var results = ValidateModel(request);
        results.Should().Contain(r => r.MemberNames.Contains("BusinessArea"));
    }

    [Fact]
    public void BusinessArea_WhenTooShort_ShouldFailValidation()
    {
        var request = CreateValidRequest();
        request.BusinessArea = "A";
        var results = ValidateModel(request);
        results.Should().Contain(r => r.MemberNames.Contains("BusinessArea"));
    }

    [Fact]
    public void BusinessArea_WhenTooLong_ShouldFailValidation()
    {
        var request = CreateValidRequest();
        request.BusinessArea = new string('A', 101);
        var results = ValidateModel(request);
        results.Should().Contain(r => r.MemberNames.Contains("BusinessArea"));
    }

    [Fact]
    public void BusinessArea_AtMinLength_ShouldPassValidation()
    {
        var request = CreateValidRequest();
        request.BusinessArea = "IT";
        var results = ValidateModel(request);
        results.Should().NotContain(r => r.MemberNames.Contains("BusinessArea"));
    }

    [Fact]
    public void CurrentChallenges_WhenEmpty_ShouldFailValidation()
    {
        var request = CreateValidRequest();
        request.CurrentChallenges = string.Empty;
        var results = ValidateModel(request);
        results.Should().Contain(r => r.MemberNames.Contains("CurrentChallenges"));
    }

    [Fact]
    public void CurrentChallenges_WhenTooShort_ShouldFailValidation()
    {
        var request = CreateValidRequest();
        request.CurrentChallenges = "Short";
        var results = ValidateModel(request);
        results.Should().Contain(r => r.MemberNames.Contains("CurrentChallenges"));
    }

    [Fact]
    public void CurrentChallenges_WhenTooLong_ShouldFailValidation()
    {
        var request = CreateValidRequest();
        request.CurrentChallenges = new string('A', 2001);
        var results = ValidateModel(request);
        results.Should().Contain(r => r.MemberNames.Contains("CurrentChallenges"));
    }

    [Fact]
    public void AvailableResources_WhenEmpty_ShouldFailValidation()
    {
        var request = CreateValidRequest();
        request.AvailableResources = string.Empty;
        var results = ValidateModel(request);
        results.Should().Contain(r => r.MemberNames.Contains("AvailableResources"));
    }

    [Fact]
    public void AvailableResources_WhenTooShort_ShouldFailValidation()
    {
        var request = CreateValidRequest();
        request.AvailableResources = "AB";
        var results = ValidateModel(request);
        results.Should().Contain(r => r.MemberNames.Contains("AvailableResources"));
    }

    [Fact]
    public void Goals_WhenEmpty_ShouldFailValidation()
    {
        var request = CreateValidRequest();
        request.Goals = string.Empty;
        var results = ValidateModel(request);
        results.Should().Contain(r => r.MemberNames.Contains("Goals"));
    }

    [Fact]
    public void Goals_WhenTooShort_ShouldFailValidation()
    {
        var request = CreateValidRequest();
        request.Goals = "AB";
        var results = ValidateModel(request);
        results.Should().Contain(r => r.MemberNames.Contains("Goals"));
    }

    [Fact]
    public void RecentMetrics_WhenEmpty_ShouldFailValidation()
    {
        var request = CreateValidRequest();
        request.RecentMetrics = string.Empty;
        var results = ValidateModel(request);
        results.Should().Contain(r => r.MemberNames.Contains("RecentMetrics"));
    }

    [Fact]
    public void RecentMetrics_WhenTooShort_ShouldFailValidation()
    {
        var request = CreateValidRequest();
        request.RecentMetrics = "Short";
        var results = ValidateModel(request);
        results.Should().Contain(r => r.MemberNames.Contains("RecentMetrics"));
    }

    [Fact]
    public void AllFieldsEmpty_ShouldFailMultipleValidations()
    {
        var request = new RecommendedActionsRequest
        {
            BusinessArea = string.Empty,
            CurrentChallenges = string.Empty,
            AvailableResources = string.Empty,
            Goals = string.Empty,
            RecentMetrics = string.Empty
        };
        var results = ValidateModel(request);
        results.Should().HaveCountGreaterOrEqualTo(5);
    }
}
