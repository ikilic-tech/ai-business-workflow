#!/bin/bash
# AI Business Workflow — API Usage Examples
#
# Prerequisites:
#   1. Start the API:  cd AiBusinessWorkflow.Api && dotnet run
#   2. Default URL:    http://localhost:5221
#
# For authenticated endpoints, add: -H "X-Api-Key: your-key"

BASE_URL="${BASE_URL:-http://localhost:5221}"

echo "=== Health Check ==="
curl -s "$BASE_URL/api/health" | python3 -m json.tool
echo ""

echo "=== AI Connection Test ==="
curl -s "$BASE_URL/api/ai/test" | python3 -m json.tool
echo ""

echo "=== AI Metrics ==="
curl -s "$BASE_URL/api/ai/metrics" | python3 -m json.tool
echo ""

echo "=== Sample Business Processes ==="
curl -s "$BASE_URL/api/samples" | python3 -m json.tool
echo ""

echo "=== Sample Customer ==="
curl -s "$BASE_URL/api/samples/customers/0" | python3 -m json.tool
echo ""

echo "=== Business Process Analysis ==="
curl -s -X POST "$BASE_URL/api/business-workflow/analyze" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Order Fulfillment",
    "description": "Customer orders received via email, manually entered into ERP, warehouse picks items, ships via courier.",
    "inputData": "Email orders, ERP system, warehouse management, courier API. 200 orders per day.",
    "goal": "Reduce order-to-ship time from 48 hours to 24 hours."
  }' | python3 -m json.tool
echo ""

echo "=== Customer Risk Assessment ==="
curl -s -X POST "$BASE_URL/api/intelligence/customer-risk" \
  -H "Content-Type: application/json" \
  -d '{
    "companyName": "Acme Industries",
    "industry": "Manufacturing",
    "employeeCount": 250,
    "annualRevenue": 15000000,
    "contactName": "Jane Smith",
    "contactEmail": "jane@acme.example.com",
    "accountAge": "3 years",
    "paymentHistory": "Generally on time, one late payment last quarter",
    "activities": [
      {"type": "Meeting", "date": "2024-01-15", "description": "Quarterly business review", "outcome": "Discussed expansion plans"},
      {"type": "Call", "date": "2024-02-20", "description": "Follow-up on new pricing", "outcome": "Requested proposal"}
    ]
  }' | python3 -m json.tool
echo ""

echo "=== Opportunity Analysis ==="
curl -s -X POST "$BASE_URL/api/intelligence/opportunity-analysis" \
  -H "Content-Type: application/json" \
  -d '{
    "accountName": "Global Logistics Co",
    "dealValue": 120000,
    "stage": "Proposal",
    "expectedCloseDate": "2024-09-30",
    "competitorInfo": "Main competitor is LogiSoft with 2-year contract ending soon",
    "notes": "CTO is supportive. Procurement team requires 3 vendor comparison.",
    "activities": [
      {"type": "Demo", "date": "2024-03-10", "description": "Full platform demo", "contactPerson": "CTO"},
      {"type": "Meeting", "date": "2024-04-05", "description": "Technical requirements review", "contactPerson": "VP Engineering"}
    ]
  }' | python3 -m json.tool
echo ""

echo "=== Activity Summary ==="
curl -s -X POST "$BASE_URL/api/intelligence/activity-summary" \
  -H "Content-Type: application/json" \
  -d '{
    "department": "Sales",
    "period": "Q1 2024",
    "activities": [
      {"employeeName": "Alice", "activityType": "Call", "date": "2024-01-10", "duration": "30 min", "description": "Prospecting call to fintech lead", "result": "Meeting booked"},
      {"employeeName": "Bob", "activityType": "Meeting", "date": "2024-01-15", "duration": "1 hour", "description": "Discovery meeting with enterprise client", "result": "Qualified opportunity"},
      {"employeeName": "Alice", "activityType": "Demo", "date": "2024-02-01", "duration": "45 min", "description": "Product demo for marketing team", "result": "Requested pricing"}
    ]
  }' | python3 -m json.tool
echo ""

echo "=== Recommended Actions ==="
curl -s -X POST "$BASE_URL/api/intelligence/recommended-actions" \
  -H "Content-Type: application/json" \
  -d '{
    "businessArea": "Customer Success",
    "currentChallenges": "Churn rate increased from 5% to 8% over last two quarters. Support ticket volume up 30%.",
    "availableResources": "4 CSMs, Zendesk, quarterly business review process, $20K training budget",
    "goals": "Reduce churn to below 5%, improve NPS from 42 to 55",
    "recentMetrics": "Q4: 8% churn, NPS 42, avg response time 4.2 hours, 85% ticket resolution within SLA"
  }' | python3 -m json.tool
echo ""

echo "=== Done ==="
