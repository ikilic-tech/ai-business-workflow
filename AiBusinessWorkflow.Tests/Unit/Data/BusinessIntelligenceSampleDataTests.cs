using AiBusinessWorkflow.Api.Data;
using FluentAssertions;

namespace AiBusinessWorkflow.Tests.Unit.Data;

public class BusinessIntelligenceSampleDataTests
{
    [Fact]
    public void GetAllCustomers_ShouldReturnNonEmptyList()
    {
        var customers = BusinessIntelligenceSampleData.GetAllCustomers();
        customers.Should().NotBeEmpty();
        customers.Should().HaveCount(3);
    }

    [Fact]
    public void GetAllCustomers_ShouldHaveUniqueIds()
    {
        var customers = BusinessIntelligenceSampleData.GetAllCustomers();
        var ids = customers.Select(c => c.CustomerId).ToList();
        ids.Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void GetAllCustomers_ShouldHaveNonEmptyFields()
    {
        var customers = BusinessIntelligenceSampleData.GetAllCustomers();
        foreach (var customer in customers)
        {
            customer.CompanyName.Should().NotBeNullOrEmpty();
            customer.Industry.Should().NotBeNullOrEmpty();
            customer.ContactName.Should().NotBeNullOrEmpty();
            customer.ContactEmail.Should().NotBeNullOrEmpty();
            customer.Activities.Should().NotBeEmpty();
        }
    }

    [Fact]
    public void GetCustomerByIndex_WithValidIndex_ShouldReturnCustomer()
    {
        var customer = BusinessIntelligenceSampleData.GetCustomerByIndex(0);
        customer.Should().NotBeNull();
        customer!.CompanyName.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void GetCustomerByIndex_WithInvalidIndex_ShouldReturnNull()
    {
        BusinessIntelligenceSampleData.GetCustomerByIndex(-1).Should().BeNull();
        BusinessIntelligenceSampleData.GetCustomerByIndex(999).Should().BeNull();
    }

    [Fact]
    public void GetAllOpportunities_ShouldReturnNonEmptyList()
    {
        var opportunities = BusinessIntelligenceSampleData.GetAllOpportunities();
        opportunities.Should().NotBeEmpty();
        opportunities.Should().HaveCount(2);
    }

    [Fact]
    public void GetAllOpportunities_ShouldHaveUniqueIds()
    {
        var opportunities = BusinessIntelligenceSampleData.GetAllOpportunities();
        var ids = opportunities.Select(o => o.OpportunityId).ToList();
        ids.Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void GetAllOpportunities_ShouldHaveNonEmptyFields()
    {
        var opportunities = BusinessIntelligenceSampleData.GetAllOpportunities();
        foreach (var opp in opportunities)
        {
            opp.AccountName.Should().NotBeNullOrEmpty();
            opp.DealValue.Should().BeGreaterThan(0);
            opp.Stage.Should().NotBeNullOrEmpty();
            opp.Activities.Should().NotBeEmpty();
        }
    }

    [Fact]
    public void GetOpportunityByIndex_WithValidIndex_ShouldReturnOpportunity()
    {
        var opportunity = BusinessIntelligenceSampleData.GetOpportunityByIndex(0);
        opportunity.Should().NotBeNull();
        opportunity!.AccountName.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void GetOpportunityByIndex_WithInvalidIndex_ShouldReturnNull()
    {
        BusinessIntelligenceSampleData.GetOpportunityByIndex(-1).Should().BeNull();
        BusinessIntelligenceSampleData.GetOpportunityByIndex(999).Should().BeNull();
    }

    [Fact]
    public void GetActivitySummary_ShouldReturnValidRequest()
    {
        var summary = BusinessIntelligenceSampleData.GetActivitySummary();
        summary.Should().NotBeNull();
        summary.Department.Should().NotBeNullOrEmpty();
        summary.Period.Should().NotBeNullOrEmpty();
        summary.Activities.Should().NotBeEmpty();
    }

    [Fact]
    public void GetActionsContext_ShouldReturnValidRequest()
    {
        var context = BusinessIntelligenceSampleData.GetActionsContext();
        context.Should().NotBeNull();
        context.BusinessArea.Should().NotBeNullOrEmpty();
        context.CurrentChallenges.Should().NotBeNullOrEmpty();
        context.AvailableResources.Should().NotBeNullOrEmpty();
        context.Goals.Should().NotBeNullOrEmpty();
        context.RecentMetrics.Should().NotBeNullOrEmpty();
    }
}
