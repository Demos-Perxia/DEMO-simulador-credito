namespace CuotaClara.Domain.Simulation;

public enum SimulationMode
{
    Amount,
    InstallmentCapacity
}

public sealed record CreditPolicy(
    decimal EffectiveAnnualRate,
    decimal MinimumAmountCop,
    decimal MaximumAmountCop,
    decimal MaximumCapacityPercentage,
    IReadOnlyList<int> TermsInMonths);

public sealed record SimulationInput(
    SimulationMode Mode,
    decimal MonthlyIncomeCop,
    decimal PayrollDeductionsCop,
    decimal? RequestedAmountCop,
    decimal? MaximumInstallmentCop);

public sealed record SimulationAlternative(
    int TermInMonths,
    decimal AmountCop,
    decimal MonthlyInstallmentCop,
    decimal TotalEstimatedCop,
    decimal EstimatedInterestCop);

public static class FrenchAmortizationCalculator
{
    public static decimal ToEffectiveMonthlyRate(decimal effectiveAnnualRate)
    {
        if (effectiveAnnualRate < 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(effectiveAnnualRate));
        }

        return (decimal)Math.Pow((double)(1m + effectiveAnnualRate), 1d / 12d) - 1m;
    }

    public static decimal CalculateInstallment(decimal principal, decimal monthlyRate, int termInMonths)
    {
        if (principal < 0m) throw new ArgumentOutOfRangeException(nameof(principal));
        if (monthlyRate < 0m) throw new ArgumentOutOfRangeException(nameof(monthlyRate));
        if (termInMonths <= 0) throw new ArgumentOutOfRangeException(nameof(termInMonths));
        if (monthlyRate == 0m) return principal / termInMonths;

        var factor = (decimal)Math.Pow((double)(1m + monthlyRate), termInMonths);
        return principal * (monthlyRate * factor) / (factor - 1m);
    }

    public static decimal CalculatePrincipal(decimal installment, decimal monthlyRate, int termInMonths)
    {
        if (installment < 0m) throw new ArgumentOutOfRangeException(nameof(installment));
        if (monthlyRate < 0m) throw new ArgumentOutOfRangeException(nameof(monthlyRate));
        if (termInMonths <= 0) throw new ArgumentOutOfRangeException(nameof(termInMonths));
        if (monthlyRate == 0m) return installment * termInMonths;

        var factor = (decimal)Math.Pow((double)(1m + monthlyRate), termInMonths);
        return installment * (factor - 1m) / (monthlyRate * factor);
    }
}

public sealed class SimulationEngine(CreditPolicy policy)
{
    public decimal EffectiveMonthlyRate => FrenchAmortizationCalculator.ToEffectiveMonthlyRate(policy.EffectiveAnnualRate);

    public IReadOnlyList<SimulationAlternative> Calculate(SimulationInput input)
    {
        Validate(input);
        var monthlyRate = EffectiveMonthlyRate;
        var alternatives = policy.TermsInMonths.Select(term => input.Mode switch
        {
            SimulationMode.Amount => FromAmount(input.RequestedAmountCop!.Value, monthlyRate, term),
            SimulationMode.InstallmentCapacity => FromInstallment(input.MaximumInstallmentCop!.Value, monthlyRate, term),
            _ => throw new InvalidOperationException("Unsupported simulation mode.")
        }).ToArray();

        if (alternatives.Any(x => x.AmountCop < policy.MinimumAmountCop || x.AmountCop > policy.MaximumAmountCop))
        {
            throw new DomainValidationException("The calculated amount is outside the configured range.");
        }

        return alternatives;
    }

    public decimal MaximumCapacity(decimal income, decimal deductions) => (income - deductions) * policy.MaximumCapacityPercentage;

    private SimulationAlternative FromAmount(decimal amount, decimal rate, int term)
    {
        var installment = FrenchAmortizationCalculator.CalculateInstallment(amount, rate, term);
        return CreateAlternative(term, amount, installment);
    }

    private SimulationAlternative FromInstallment(decimal installment, decimal rate, int term)
    {
        var amount = FrenchAmortizationCalculator.CalculatePrincipal(installment, rate, term);
        return CreateAlternative(term, amount, installment);
    }

    private static SimulationAlternative CreateAlternative(int term, decimal amount, decimal installment)
    {
        var roundedAmount = RoundCop(amount);
        var roundedInstallment = RoundCop(installment);
        var total = RoundCop(installment * term);
        return new SimulationAlternative(term, roundedAmount, roundedInstallment, total, total - roundedAmount);
    }

    private void Validate(SimulationInput input)
    {
        if (input.MonthlyIncomeCop <= 0m) throw new DomainValidationException("Monthly income must be positive.");
        if (input.PayrollDeductionsCop < 0m || input.PayrollDeductionsCop >= input.MonthlyIncomeCop) throw new DomainValidationException("Payroll deductions must be non-negative and lower than income.");

        var capacity = MaximumCapacity(input.MonthlyIncomeCop, input.PayrollDeductionsCop);
        if (input.Mode == SimulationMode.Amount)
        {
            if (input.RequestedAmountCop is null || input.MaximumInstallmentCop is not null) throw new DomainValidationException("Amount mode requires only requested amount.");
            if (input.RequestedAmountCop < policy.MinimumAmountCop || input.RequestedAmountCop > policy.MaximumAmountCop) throw new DomainValidationException("Requested amount is outside the configured range.");
            if (policy.TermsInMonths.Any(term => FrenchAmortizationCalculator.CalculateInstallment(input.RequestedAmountCop.Value, EffectiveMonthlyRate, term) > capacity)) throw new DomainValidationException("The resulting installment exceeds payment capacity.");
        }
        else if (input.Mode == SimulationMode.InstallmentCapacity)
        {
            if (input.MaximumInstallmentCop is null || input.RequestedAmountCop is not null) throw new DomainValidationException("Installment capacity mode requires only maximum installment.");
            if (input.MaximumInstallmentCop <= 0m || input.MaximumInstallmentCop > capacity) throw new DomainValidationException("Maximum installment exceeds payment capacity.");
        }
        else throw new DomainValidationException("Simulation mode is invalid.");
    }

    private static decimal RoundCop(decimal value) => decimal.Round(value, 0, MidpointRounding.AwayFromZero);
}

public sealed class DomainValidationException(string message) : Exception(message);
