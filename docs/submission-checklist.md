# Submission Checklist

## Project ZIP Archive

- Complete runnable ASP.NET Core solution included.
- API project included: `ProposalCompliance.Api`.
- Test project included: `ProposalCompliance.Tests`.
- Build configuration included: `.editorconfig`, `Directory.Build.props`, `NuGet.config`.
- Generated folders are excluded by `.gitignore`.

## Technical Report PDF

- Cover page and table of contents included.
- Introduction included.
- System overview diagram included.
- Model and validation explanation included.
- Compliance rules explained.
- Endpoint and response examples included.
- Setup instructions included.
- Six manual test cases included.
- Key findings and conclusion included.

## Verification

- `dotnet build ProposalCompliance.sln --no-restore`
- `dotnet test ProposalCompliance.sln --no-build`
