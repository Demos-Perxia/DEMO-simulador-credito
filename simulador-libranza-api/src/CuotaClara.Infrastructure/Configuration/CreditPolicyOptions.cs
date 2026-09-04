using CuotaClara.Application.Abstractions;
using CuotaClara.Domain.Simulation;
using Microsoft.Extensions.Options;

namespace CuotaClara.Infrastructure.Configuration;

public sealed class CreditPolicyOptions
{
    public const string SectionName = "CreditPolicy";
    public decimal EffectiveAnnualRate { get; init; } = 0.18m;
    public decimal MinimumAmountCop { get; init; } = 1_000_000m;
    public decimal MaximumAmountCop { get; init; } = 100_000_000m;
    public decimal MaximumCapacityPercentage { get; init; } = 0.40m;
    public int[] TermsInMonths { get; set; } = [];
}

public sealed class ConfiguredCreditPolicyProvider(IOptions<CreditPolicyOptions> options) : ICreditPolicyProvider
{
    public CreditPolicy Get()
    {
        var value = options.Value;
        var terms = value.TermsInMonths.Length == 0 ? [60, 72, 96, 108, 120] : value.TermsInMonths.Distinct().OrderBy(term => term).ToArray();
        return new CreditPolicy(value.EffectiveAnnualRate, value.MinimumAmountCop, value.MaximumAmountCop, value.MaximumCapacityPercentage, terms);
    }
}
