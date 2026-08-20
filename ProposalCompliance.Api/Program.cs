using Microsoft.AspNetCore.Mvc;
using ProposalCompliance.Api.Contracts;
using ProposalCompliance.Api.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(entry => entry.Value?.Errors.Count > 0)
                .SelectMany(entry => entry.Value!.Errors.Select(error => new ValidationIssue(
                    Field: entry.Key,
                    Message: error.ErrorMessage)))
                .OrderBy(error => error.Field, StringComparer.Ordinal)
                .ToArray();

            var response = ProposalComplianceResponse.ValidationFailed(errors);

            return new BadRequestObjectResult(response);
        };
    });

builder.Services.AddScoped<IComplianceAnalysisService, ComplianceAnalysisService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();

public partial class Program;
