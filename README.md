# PAS Proposal Compliance Service

Submission-ready ASP.NET Core Web API and premium web console for the PAS initial proposal screening workflow. The service receives a project proposal, validates the submitted data, and returns a consistent compliance decision with any rule-based alerts.

## What Is Included

- ASP.NET Core Web API with controller-based endpoint design.
- Premium browser UI served from the API project root.
- Server-side model validation using Data Annotations.
- Separate compliance analysis service for clean rule ownership.
- Structured response envelope for validation errors and compliance alerts.
- Automated unit and API tests.
- Coursework technical report in `output/pdf/`.

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
- Frontend is self-contained, responsive, and does not require external CSS or JavaScript packages.

## Run The Application

```bash
dotnet run --project ProposalCompliance.Api
```

Open the URL shown in the terminal. The UI is available at `/`.

## Check Commands

```bash
dotnet restore ProposalCompliance.sln
dotnet build ProposalCompliance.sln --no-restore
dotnet test ProposalCompliance.sln --no-build
```

## Submission Files

- `ProposalCompliance.sln`
- `ProposalCompliance.Api/`
- `ProposalCompliance.Tests/`
- `README.md`
- `output/pdf/PAS_Proposal_Compliance_Service_Technical_Report.pdf`
