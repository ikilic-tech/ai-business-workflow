"""
AI Business Workflow — Python Client Example

This script demonstrates how to call the AI Business Workflow API
from Python. It can be used as a starting point for integrations.

Prerequisites:
    pip install requests

Usage:
    python python-client.py
"""

import requests
import json
import sys

BASE_URL = "http://localhost:5221"
API_KEY = None  # Set if authentication is enabled


def headers():
    h = {"Content-Type": "application/json"}
    if API_KEY:
        h["X-Api-Key"] = API_KEY
    return h


def print_json(data):
    print(json.dumps(data, indent=2, ensure_ascii=False))


def health_check():
    print("=== Health Check ===")
    r = requests.get(f"{BASE_URL}/api/health")
    print(f"Status: {r.status_code}")
    print_json(r.json())
    print()


def ai_metrics():
    print("=== AI Metrics ===")
    r = requests.get(f"{BASE_URL}/api/ai/metrics", headers=headers())
    print(f"Status: {r.status_code}")
    print_json(r.json())
    print()


def analyze_business_process():
    print("=== Business Process Analysis ===")
    payload = {
        "name": "Order Fulfillment",
        "description": (
            "Customer orders received via email, manually entered into ERP, "
            "warehouse picks items, ships via courier."
        ),
        "inputData": (
            "Email orders, ERP system, warehouse management, courier API. "
            "200 orders per day."
        ),
        "goal": "Reduce order-to-ship time from 48 hours to 24 hours.",
    }
    r = requests.post(
        f"{BASE_URL}/api/business-workflow/analyze",
        headers=headers(),
        json=payload,
    )
    print(f"Status: {r.status_code}")
    if r.ok:
        data = r.json()
        print(f"Efficiency: {data['efficiency']['score']}/100 ({data['efficiency']['rating']})")
        print(f"Bottlenecks: {len(data.get('bottlenecks', []))}")
        print(f"Recommendations: {len(data.get('recommendations', []))}")
        print(f"Summary: {data.get('summary', 'N/A')}")
    else:
        print_json(r.json())
    print()


def assess_customer_risk():
    print("=== Customer Risk Assessment ===")
    payload = {
        "companyName": "Acme Industries",
        "industry": "Manufacturing",
        "employeeCount": 250,
        "annualRevenue": 15000000,
        "contactName": "Jane Smith",
        "contactEmail": "jane@acme.example.com",
        "accountAge": "3 years",
        "paymentHistory": "Generally on time, one late payment last quarter",
        "activities": [
            {
                "type": "Meeting",
                "date": "2024-01-15",
                "description": "Quarterly business review",
                "outcome": "Discussed expansion plans",
            },
            {
                "type": "Call",
                "date": "2024-02-20",
                "description": "Follow-up on new pricing",
                "outcome": "Requested proposal",
            },
        ],
    }
    r = requests.post(
        f"{BASE_URL}/api/intelligence/customer-risk",
        headers=headers(),
        json=payload,
    )
    print(f"Status: {r.status_code}")
    if r.ok:
        data = r.json()
        print(f"Risk Score: {data['riskScore']}/100")
        print(f"Risk Level: {data['riskLevel']}")
        print(f"Churn Probability: {data['churnProbability']}")
        print(f"Engagement Trend: {data['engagementTrend']}")
        print(f"Summary: {data.get('summary', 'N/A')}")
    else:
        print_json(r.json())
    print()


def analyze_opportunity():
    print("=== Opportunity Analysis ===")
    payload = {
        "accountName": "Global Logistics Co",
        "dealValue": 120000,
        "stage": "Proposal",
        "expectedCloseDate": "2024-09-30",
        "competitorInfo": "Main competitor is LogiSoft with 2-year contract ending soon",
        "notes": "CTO is supportive. Procurement requires 3 vendor comparison.",
        "activities": [
            {
                "type": "Demo",
                "date": "2024-03-10",
                "description": "Full platform demo",
                "contactPerson": "CTO",
            },
            {
                "type": "Meeting",
                "date": "2024-04-05",
                "description": "Technical requirements review",
                "contactPerson": "VP Engineering",
            },
        ],
    }
    r = requests.post(
        f"{BASE_URL}/api/intelligence/opportunity-analysis",
        headers=headers(),
        json=payload,
    )
    print(f"Status: {r.status_code}")
    if r.ok:
        data = r.json()
        print(f"Win Probability: {data['winProbability']}%")
        print(f"Verdict: {data['verdict']}")
        print(f"Competitive Position: {data.get('competitivePosition', 'N/A')}")
        print(f"Summary: {data.get('summary', 'N/A')}")
    else:
        print_json(r.json())
    print()


def run_dashboard():
    print("=== Management Dashboard ===")
    payload = {
        "customer": {
            "companyName": "TechCorp",
            "industry": "Technology",
            "employeeCount": 200,
            "annualRevenue": 10000000,
            "contactName": "John Doe",
            "contactEmail": "john@techcorp.example.com",
            "accountAge": "2 years",
            "paymentHistory": "Always on time, excellent track record",
            "activities": [
                {
                    "type": "Meeting",
                    "date": "2024-01-15",
                    "description": "Quarterly review",
                    "outcome": "Positive",
                }
            ],
        },
        "actionsContext": {
            "businessArea": "Sales",
            "currentChallenges": "Pipeline velocity has decreased over the past quarter",
            "availableResources": "8 sales reps, CRM platform",
            "goals": "Increase pipeline velocity by 20%",
            "recentMetrics": "Q1: Revenue $1.5M against $2M target",
        },
    }
    r = requests.post(
        f"{BASE_URL}/api/intelligence/dashboard",
        headers=headers(),
        json=payload,
    )
    print(f"Status: {r.status_code}")
    if r.ok:
        data = r.json()
        print(f"Customer Risk: {'✓' if data.get('customerRisk') else '—'}")
        print(f"Activity Summary: {'✓' if data.get('activitySummary') else '—'}")
        print(f"Opportunity Analysis: {'✓' if data.get('opportunityAnalysis') else '—'}")
        print(f"Recommended Actions: {'✓' if data.get('recommendedActions') else '—'}")
    else:
        print_json(r.json())
    print()


if __name__ == "__main__":
    if len(sys.argv) > 1:
        BASE_URL = sys.argv[1]
    if len(sys.argv) > 2:
        API_KEY = sys.argv[2]

    print(f"Target: {BASE_URL}\n")

    health_check()
    ai_metrics()
    assess_customer_risk()
    analyze_opportunity()
    analyze_business_process()
    run_dashboard()

    print("=== Done ===")
