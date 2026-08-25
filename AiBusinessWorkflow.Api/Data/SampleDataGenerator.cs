using AiBusinessWorkflow.Api.Models;

namespace AiBusinessWorkflow.Api.Data;

public static class SampleDataGenerator
{
    private static readonly List<BusinessProcess> Samples = new()
    {
        new BusinessProcess
        {
            Id = "sample-001",
            Name = "Customer Onboarding",
            Description = "End-to-end process for registering new B2B customers, including KYC verification, account setup, CRM record creation, and welcome email sequence.",
            InputData = "Company name: Acme Corp, Contact: Jane Smith (jane@acme.example.com), Industry: Manufacturing, Employees: 250, Annual Revenue: $45M, Documents: Business registration certificate, Tax ID, Bank details",
            Goal = "Reduce onboarding time from 5 business days to 2 business days while maintaining compliance requirements and improving first-week engagement rates."
        },
        new BusinessProcess
        {
            Id = "sample-002",
            Name = "Invoice Processing",
            Description = "Accounts payable workflow covering invoice receipt, data extraction, three-way matching (PO, receipt, invoice), approval routing, and payment scheduling.",
            InputData = "Monthly volume: 1200 invoices, Average processing time: 8 days, Error rate: 12%, Manual data entry: 85% of invoices, Approval levels: 3 (manager, director, VP for >$10K), Current tools: Email + Excel spreadsheets",
            Goal = "Achieve 80% straight-through processing rate, reduce average cycle time to 3 days, and bring error rate below 2%."
        },
        new BusinessProcess
        {
            Id = "sample-003",
            Name = "Sales Lead Qualification",
            Description = "Process for evaluating inbound marketing leads, scoring them based on firmographic and behavioral data, routing qualified leads to sales reps, and tracking conversion through the pipeline.",
            InputData = "Monthly inbound leads: 850, Current qualification rate: 15%, Average response time: 48 hours, Lead sources: Website forms (40%), Trade shows (25%), Referrals (20%), Cold outreach (15%), CRM: Salesforce, Sales team size: 12 reps across 3 regions",
            Goal = "Increase qualified lead conversion rate to 25%, reduce initial response time to under 4 hours, and improve sales rep productivity by eliminating manual lead research."
        },
        new BusinessProcess
        {
            Id = "sample-004",
            Name = "IT Support Ticket Resolution",
            Description = "Help desk workflow from ticket creation through triage, assignment, resolution, and user satisfaction survey. Covers L1 through L3 support tiers with escalation paths.",
            InputData = "Monthly tickets: 2400, Categories: Password resets (30%), Software issues (25%), Hardware (20%), Network (15%), Access requests (10%), Average resolution time: L1=4hrs L2=18hrs L3=72hrs, SLA compliance: 78%, Team size: 8 L1, 4 L2, 2 L3 engineers",
            Goal = "Achieve 90% SLA compliance, automate 40% of L1 tickets (password resets and common software issues), and reduce mean time to resolution across all tiers by 30%."
        },
        new BusinessProcess
        {
            Id = "sample-005",
            Name = "Supply Chain Order Fulfillment",
            Description = "Order-to-delivery process including inventory check, warehouse picking, quality inspection, packing, carrier selection, shipping, and delivery confirmation with returns handling.",
            InputData = "Daily orders: 350, Warehouse locations: 3, SKU count: 8500, Average fulfillment time: 3.2 days, Order accuracy: 96.5%, Return rate: 8%, Carrier partners: 4 (FedEx, UPS, DHL, regional), Peak season multiplier: 2.5x (Nov-Dec)",
            Goal = "Reduce fulfillment time to under 24 hours for standard orders, achieve 99.5% order accuracy, and decrease return rate to 5% through better quality checks."
        },
        new BusinessProcess
        {
            Id = "sample-006",
            Name = "Employee Performance Review",
            Description = "Quarterly performance evaluation cycle including self-assessment, peer feedback collection, manager review, calibration sessions, and development plan creation.",
            InputData = "Employee count: 180, Review frequency: Quarterly, Current completion rate: 65% on time, Average time per review: 3 hours (manager), Feedback sources: Self (100%), Peers (avg 3 per employee), Manager (1), Tools: Google Forms + manual compilation, Pain points: Inconsistent rating scales, recency bias, delayed feedback",
            Goal = "Increase on-time completion rate to 95%, reduce manager time per review to 1 hour through pre-populated insights, and implement continuous feedback to reduce recency bias."
        }
    };

    public static IReadOnlyList<BusinessProcess> GetAll() => Samples;

    public static BusinessProcess? GetByIndex(int index)
    {
        if (index < 0 || index >= Samples.Count)
            return null;

        return Samples[index];
    }
}
