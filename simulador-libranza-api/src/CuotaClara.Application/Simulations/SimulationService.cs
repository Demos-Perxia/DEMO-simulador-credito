using CuotaClara.Application.Abstractions;
using CuotaClara.Application.Catalogs;
using CuotaClara.Domain.Simulation;

namespace CuotaClara.Application.Simulations;

public sealed record CreateSimulationCommand(string ActivityId, string AgreementId, SimulationMode Mode, decimal MonthlyIncomeCop, decimal PayrollDeductionsCop, decimal? RequestedAmountCop, decimal? MaximumInstallmentCop);
public sealed record SimulationResult(decimal EffectiveAnnualRate, decimal EffectiveMonthlyRate, decimal MaximumPaymentCapacityCop, IReadOnlyList<SimulationAlternative> Alternatives);

public sealed class SimulationService(ICatalogRepository catalogs, ICreditPolicyProvider policyProvider)
{
    public SimulationResult Create(CreateSimulationCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.ActivityId) || !catalogs.ActivityExists(command.ActivityId)) throw new ApplicationValidationException("Activity was not found.");
        if (string.IsNullOrWhiteSpace(command.AgreementId) || !catalogs.AgreementIsCompatible(command.ActivityId, command.AgreementId)) throw new ApplicationValidationException("Agreement is not compatible with the activity.");

        var policy = policyProvider.Get();
        var engine = new SimulationEngine(policy);
        var input = new SimulationInput(command.Mode, command.MonthlyIncomeCop, command.PayrollDeductionsCop, command.RequestedAmountCop, command.MaximumInstallmentCop);
        var alternatives = engine.Calculate(input);
        return new SimulationResult(policy.EffectiveAnnualRate, engine.EffectiveMonthlyRate, decimal.Round(engine.MaximumCapacity(command.MonthlyIncomeCop, command.PayrollDeductionsCop), 0, MidpointRounding.AwayFromZero), alternatives);
    }
}
