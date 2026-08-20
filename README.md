# Proposal Compliance Service

Premium ASP.NET Core Web API for the PAS initial proposal screening workflow. The service receives a project proposal, validates the submitted data, and returns a consistent compliance decision with any rule-based alerts.

## Endpoint

`POST /api/proposal/analyse`

```json
{
  "studentName": "Navida Perera",
  "resourceRequested": "Advanced GPU Cluster",
  "quantity": 4,
  "ethicsRiskCountry": "Bermuda",
  "estimatedBudgetUSD": 9000,
  "researchArea": "Applied AI"
}
```

## Response Shape

```json
{
  "complianceStatus": "ReviewRequired",
  "validationErrors": [],
  "complianceAlerts": [
    {
      "ruleCode": "BUDGET-001",
      "severity": "Low",
      "message": "High funding limit detected. Requires manual academic coordinator review."
    }
  ]
}
```

## Quality Notes

- Controller remains thin; compliance rules are isolated in `IComplianceAnalysisService`.
- Request validation uses Data Annotations on `ProposalRequest`.
- Invalid submissions return a stable `ValidationFailed` response instead of default framework error noise.
- Automated tests cover the successful path, validation failures, all three coursework rules, and combined alerts.

## Check Commands

```bash
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```
