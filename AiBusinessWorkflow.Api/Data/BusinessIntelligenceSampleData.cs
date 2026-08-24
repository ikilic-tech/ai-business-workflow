using AiBusinessWorkflow.Api.Models;

namespace AiBusinessWorkflow.Api.Data;

public static class BusinessIntelligenceSampleData
{
    private static readonly List<CustomerProfile> Customers = new()
    {
        new CustomerProfile
        {
            CustomerId = "cust-001",
            CompanyName = "TechFlow Solutions",
            Industry = "Technology",
            EmployeeCount = 450,
            AnnualRevenue = 85000000m,
            ContactName = "Sarah Chen",
            ContactEmail = "sarah.chen@techflow.com",
            AccountAge = "4 years",
            PaymentHistory = "Consistently on time, no missed payments in 4 years. Upgraded plan twice. Average monthly spend increased 35% year over year.",
            Activities = new List<CustomerActivity>
            {
                new() { Type = "Support Ticket", Date = "2024-01-15", Description = "Requested help with API integration for new module", Outcome = "Resolved in 2 hours" },
                new() { Type = "Meeting", Date = "2024-01-22", Description = "Quarterly business review with account team", Outcome = "Discussed expansion to 3 new departments" },
                new() { Type = "Purchase", Date = "2024-02-01", Description = "Added 50 additional user licenses", Outcome = "Deal closed same day" },
                new() { Type = "Training", Date = "2024-02-10", Description = "Onboarding session for new department users", Outcome = "25 users trained successfully" },
                new() { Type = "Feedback", Date = "2024-03-01", Description = "Submitted NPS survey", Outcome = "Score: 9/10, highlighted ease of use" }
            }
        },
        new CustomerProfile
        {
            CustomerId = "cust-002",
            CompanyName = "Global Retail Partners",
            Industry = "Retail",
            EmployeeCount = 180,
            AnnualRevenue = 32000000m,
            ContactName = "Michael Torres",
            ContactEmail = "m.torres@globalretail.com",
            AccountAge = "2 years",
            PaymentHistory = "Generally on time, 2 late payments in past year (15 and 22 days late). Maintained same plan level since signup.",
            Activities = new List<CustomerActivity>
            {
                new() { Type = "Support Ticket", Date = "2024-01-10", Description = "Reported dashboard loading slowly during peak hours", Outcome = "Escalated to engineering, fix pending" },
                new() { Type = "Meeting", Date = "2024-01-20", Description = "Account review call, customer raised concerns about ROI", Outcome = "Action items assigned to account team" },
                new() { Type = "Support Ticket", Date = "2024-02-05", Description = "Integration with POS system failing intermittently", Outcome = "Workaround provided, root cause under investigation" },
                new() { Type = "Email", Date = "2024-02-15", Description = "Customer asked about contract renewal terms and pricing", Outcome = "Pricing proposal sent" }
            }
        },
        new CustomerProfile
        {
            CustomerId = "cust-003",
            CompanyName = "NorthStar Logistics",
            Industry = "Logistics",
            EmployeeCount = 95,
            AnnualRevenue = 18000000m,
            ContactName = "Lisa Park",
            ContactEmail = "lisa.park@northstarlogistics.com",
            AccountAge = "8 months",
            PaymentHistory = "3 late payments out of 8 invoices. Downgraded from Pro to Basic plan 2 months ago. Disputed one invoice.",
            Activities = new List<CustomerActivity>
            {
                new() { Type = "Support Ticket", Date = "2024-01-05", Description = "Complained about lack of fleet tracking features", Outcome = "Feature request logged, no ETA given" },
                new() { Type = "Support Ticket", Date = "2024-01-18", Description = "Billing dispute on December invoice", Outcome = "Credit issued after review" },
                new() { Type = "Cancellation Inquiry", Date = "2024-02-01", Description = "Asked about early termination terms for annual contract", Outcome = "Retention offer made, customer said they would think about it" },
                new() { Type = "Email", Date = "2024-02-20", Description = "No response to follow-up from account manager", Outcome = "Unresponsive" }
            }
        }
    };

    private static readonly List<Opportunity> Opportunities = new()
    {
        new Opportunity
        {
            OpportunityId = "opp-001",
            AccountName = "Meridian Healthcare Group",
            DealValue = 250000m,
            Stage = "Proposal Sent",
            ExpectedCloseDate = "2024-04-30",
            CompetitorInfo = "Competing against HealthTech Pro (incumbent, 3-year relationship) and MedFlow Systems (lower price point, fewer features). Meridian has expressed frustration with HealthTech Pro's support response times.",
            Notes = "Champion is the CTO, Dr. Patel. CFO is cautious about switching costs. Need to demonstrate ROI within 6 months. Security compliance (HIPAA) is a hard requirement.",
            Activities = new List<OpportunityActivity>
            {
                new() { Type = "Discovery Call", Date = "2024-01-10", Description = "Initial needs assessment with IT director and department heads", ContactPerson = "James Wright, IT Director" },
                new() { Type = "Demo", Date = "2024-01-24", Description = "Full platform demo with clinical workflow scenarios", ContactPerson = "Dr. Ananya Patel, CTO" },
                new() { Type = "Technical Review", Date = "2024-02-05", Description = "Security and compliance review with IT security team", ContactPerson = "Robert Kim, CISO" },
                new() { Type = "Proposal", Date = "2024-02-15", Description = "Submitted formal proposal with 3-year pricing and implementation plan", ContactPerson = "Dr. Ananya Patel, CTO" }
            }
        },
        new Opportunity
        {
            OpportunityId = "opp-002",
            AccountName = "Summit Financial Advisors",
            DealValue = 85000m,
            Stage = "Negotiation",
            ExpectedCloseDate = "2024-03-15",
            CompetitorInfo = "Main competitor is FinanceHub (similar pricing, stronger brand recognition in financial sector). Summit has used our product in a trial but adoption was slow. Decision maker prefers FinanceHub's reporting module.",
            Notes = "Deal has been in negotiation for 6 weeks. Customer pushing for 30% discount. Our champion (VP of Operations) left the company last month. New contact is evaluating all vendor relationships.",
            Activities = new List<OpportunityActivity>
            {
                new() { Type = "Trial Setup", Date = "2023-11-01", Description = "Set up 30-day trial with 10 user licenses", ContactPerson = "David Okafor, VP Operations" },
                new() { Type = "Check-in", Date = "2023-11-20", Description = "Trial usage review, only 4 of 10 users active", ContactPerson = "David Okafor, VP Operations" },
                new() { Type = "Negotiation", Date = "2024-01-15", Description = "Pricing discussion, customer requesting significant discount", ContactPerson = "Karen Liu, Procurement" },
                new() { Type = "Meeting", Date = "2024-02-10", Description = "Meeting with new VP to re-establish relationship and value proposition", ContactPerson = "Thomas Reed, VP Operations (new)" }
            }
        }
    };

    private static readonly ActivitySummaryRequest SampleActivitySummary = new()
    {
        Department = "Sales",
        Period = "Q1 2024",
        Activities = new List<ActivityEntry>
        {
            new() { EmployeeName = "Alice Johnson", ActivityType = "Cold Call", Date = "2024-01-08", Duration = "25 minutes", Description = "Outbound call to Pinnacle Corp regarding enterprise license", Result = "Meeting scheduled for Jan 15" },
            new() { EmployeeName = "Alice Johnson", ActivityType = "Demo", Date = "2024-01-15", Duration = "60 minutes", Description = "Product demonstration for Pinnacle Corp decision makers", Result = "Positive reception, requested proposal" },
            new() { EmployeeName = "Bob Martinez", ActivityType = "Email Campaign", Date = "2024-01-10", Duration = "3 hours", Description = "Launched targeted email campaign to 200 mid-market prospects", Result = "18% open rate, 12 responses, 4 meetings booked" },
            new() { EmployeeName = "Bob Martinez", ActivityType = "Networking Event", Date = "2024-01-22", Duration = "4 hours", Description = "Attended TechConnect conference, manned booth", Result = "Collected 45 leads, 8 qualified" },
            new() { EmployeeName = "Carol Williams", ActivityType = "Account Review", Date = "2024-01-18", Duration = "90 minutes", Description = "Reviewed 15 stale opportunities in pipeline for re-engagement", Result = "5 accounts flagged for follow-up, 3 marked as lost" },
            new() { EmployeeName = "Carol Williams", ActivityType = "Client Visit", Date = "2024-02-05", Duration = "3 hours", Description = "On-site meeting with Vertex Industries for contract renewal", Result = "Renewal confirmed with 10% upsell" },
            new() { EmployeeName = "David Lee", ActivityType = "Proposal Writing", Date = "2024-02-12", Duration = "5 hours", Description = "Prepared custom proposal for Metro Health Systems RFP", Result = "Submitted on time, shortlisted" },
            new() { EmployeeName = "David Lee", ActivityType = "Follow-up Call", Date = "2024-02-20", Duration = "30 minutes", Description = "Follow-up with Metro Health on proposal questions", Result = "Clarified pricing structure, final decision expected in March" },
            new() { EmployeeName = "Alice Johnson", ActivityType = "Training", Date = "2024-03-01", Duration = "2 hours", Description = "Led new hire training on CRM usage and sales playbook", Result = "3 new reps trained on system and process" },
            new() { EmployeeName = "Bob Martinez", ActivityType = "Cold Call", Date = "2024-03-05", Duration = "45 minutes", Description = "Called 12 prospects from conference lead list", Result = "3 interested, 2 meetings scheduled" }
        }
    };

    private static readonly RecommendedActionsRequest SampleActionsContext = new()
    {
        BusinessArea = "Sales Operations",
        CurrentChallenges = "Sales cycle has lengthened from 45 to 68 days over the past two quarters. Win rate dropped from 32% to 24%. Top performers are spending 40% of their time on administrative tasks. CRM data quality is inconsistent with 30% of records missing key fields. New hire ramp-up time is averaging 6 months versus industry standard of 3-4 months.",
        AvailableResources = "12 sales reps across 3 regions, 2 sales operations analysts, CRM platform (Salesforce), marketing automation tool, $50K quarterly budget for tools and training, executive sponsorship for process improvement.",
        Goals = "Reduce sales cycle to under 50 days, improve win rate to 30%, free up 15+ hours per rep per month from admin work, achieve 95% CRM data completeness, reduce new hire ramp to 4 months.",
        RecentMetrics = "Q4 2023: Revenue $2.1M (target $2.5M, 84% attainment). Q1 2024 pipeline: $8.2M total, $3.1M in late stages. Average deal size: $45K (down from $52K). Lead response time: 6.2 hours average. Customer acquisition cost: $12,400 (up 18% YoY). Quota attainment across team: 4 of 12 reps at or above quota."
    };

    public static IReadOnlyList<CustomerProfile> GetAllCustomers() => Customers;

    public static CustomerProfile? GetCustomerByIndex(int index)
    {
        if (index < 0 || index >= Customers.Count)
            return null;

        return Customers[index];
    }

    public static IReadOnlyList<Opportunity> GetAllOpportunities() => Opportunities;

    public static Opportunity? GetOpportunityByIndex(int index)
    {
        if (index < 0 || index >= Opportunities.Count)
            return null;

        return Opportunities[index];
    }

    public static ActivitySummaryRequest GetActivitySummary() => SampleActivitySummary;

    public static RecommendedActionsRequest GetActionsContext() => SampleActionsContext;
}
