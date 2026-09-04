using System.Text.Json;
using CuotaClara.Api.Contracts;
using CuotaClara.Application.Catalogs;
using CuotaClara.Application.Simulations;
using CuotaClara.Domain.Simulation;
using Microsoft.AspNetCore.Mvc;

namespace CuotaClara.Api.Endpoints;

public static class ApiEndpoints
{
    public static IEndpointRouteBuilder MapCuotaClaraEndpoints(this IEndpointRouteBuilder routes)
    {
        var catalogs = routes.MapGroup("/api/v1/catalogs").WithTags("Catalogs");
        catalogs.MapGet("/activities", (CatalogService service) => Results.Ok(service.GetActivities().Select(x => new ActivityResponse(x.Id, x.Name))));
        catalogs.MapGet("/activities/{activityId}/agreements", (string activityId, CatalogService service) => Results.Ok(service.GetAgreements(activityId).Select(x => new AgreementResponse(x.Id, x.Name))));

        routes.MapPost("/api/v1/credit-simulations", ([FromBody] CreateSimulationRequest request, SimulationService service) =>
        {
            if (!RequestMappings.TryMapMode(request.Mode, out var mode)) throw new ApplicationValidationException("Mode must be AMOUNT or INSTALLMENT_CAPACITY.");
            if (request.MonthlyIncomeCop is null || request.PayrollDeductionsCop is null) throw new ApplicationValidationException("Monthly income and payroll deductions are required.");
            var result = service.Create(new CreateSimulationCommand(request.ActivityId ?? string.Empty, request.AgreementId ?? string.Empty, mode, request.MonthlyIncomeCop.Value, request.PayrollDeductionsCop.Value, request.RequestedAmountCop, request.MaximumInstallmentCop));
            var response = new SimulationResponse(request.Mode!.ToUpperInvariant(), result.EffectiveAnnualRate, result.EffectiveMonthlyRate, result.MaximumPaymentCapacityCop, result.Alternatives.Select(x => new SimulationAlternativeResponse(x.TermInMonths, x.AmountCop, x.MonthlyInstallmentCop, x.TotalEstimatedCop, x.EstimatedInterestCop)).ToArray(), "Resultado informativo: no constituye aprobación de crédito y no incluye seguro de vida.");
            return Results.Ok(response);
        }).WithTags("Credit simulations").Accepts<CreateSimulationRequest>("application/json").Produces<SimulationResponse>().ProducesProblem(StatusCodes.Status400BadRequest);
        return routes;
    }

    public static IApplicationBuilder UseProblemDetails(this IApplicationBuilder app)
    {
        return app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
        {
            var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
            var isValidation = exception is ApplicationValidationException or DomainValidationException;
            var status = isValidation ? StatusCodes.Status400BadRequest : StatusCodes.Status500InternalServerError;
            var problem = new ProblemDetails { Status = status, Title = isValidation ? "Validation failed" : "Unexpected error", Detail = isValidation ? exception!.Message : "An unexpected error occurred." };
            context.Response.StatusCode = status;
            context.Response.ContentType = "application/problem+json";
            await JsonSerializer.SerializeAsync(context.Response.Body, problem, cancellationToken: context.RequestAborted);
        }));
    }
}
