using CuotaClara.Domain.Simulation;

namespace CuotaClara.Api.Contracts;

public sealed record ActivityResponse(string Id, string Name);
public sealed record AgreementResponse(string Id, string Name);
public sealed record CreateSimulationRequest(string? ActivityId, string? AgreementId, string? Mode, decimal? MonthlyIncomeCop, decimal? PayrollDeductionsCop, decimal? RequestedAmountCop, decimal? MaximumInstallmentCop);
public sealed record SimulationAlternativeResponse(int TermInMonths, decimal AmountCop, decimal MonthlyInstallmentCop, decimal TotalEstimatedCop, decimal EstimatedInterestCop);
public sealed record SimulationResponse(string Mode, decimal EffectiveAnnualRate, decimal EffectiveMonthlyRate, decimal MaximumPaymentCapacityCop, IReadOnlyList<SimulationAlternativeResponse> Alternatives, string Disclaimer);

public static class RequestMappings
{
    public static bool TryMapMode(string? value, out SimulationMode mode)
    {
        if (string.Equals(value, "AMOUNT", StringComparison.OrdinalIgnoreCase)) { mode = SimulationMode.Amount; return true; }
        if (string.Equals(value, "INSTALLMENT_CAPACITY", StringComparison.OrdinalIgnoreCase)) { mode = SimulationMode.InstallmentCapacity; return true; }
        mode = default;
        return false;
    }
}
