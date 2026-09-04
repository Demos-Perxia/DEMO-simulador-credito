using CuotaClara.Domain.Simulation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CuotaClara.Domain.Tests;

[TestClass]
public sealed class FrenchAmortizationCalculatorTests
{
    private static readonly CreditPolicy Policy = new(0.18m, 1_000_000m, 100_000_000m, 0.40m, [60, 72, 96, 108, 120]);

    [TestMethod]
    public void Converts_Effective_Annual_Rate_To_A_Positive_Monthly_Rate()
    {
        Assert.IsTrue(FrenchAmortizationCalculator.ToEffectiveMonthlyRate(0.18m) > 0m);
    }

    [TestMethod]
    public void Calculates_Zero_Rate_Installment()
    {
        Assert.AreEqual(100m, FrenchAmortizationCalculator.CalculateInstallment(1_200m, 0m, 12));
    }

    [TestMethod]
    public void Amount_Mode_Returns_All_Terms_With_Coherent_Alternatives()
    {
        var alternatives = new SimulationEngine(Policy).Calculate(new SimulationInput(SimulationMode.Amount, 10_000_000m, 1_000_000m, 20_000_000m, null));
        Assert.AreEqual(5, alternatives.Count);
        Assert.IsTrue(alternatives[4].MonthlyInstallmentCop <= alternatives[0].MonthlyInstallmentCop);
        Assert.IsTrue(alternatives[4].EstimatedInterestCop >= alternatives[0].EstimatedInterestCop);
    }

    [TestMethod]
    public void Installment_Capacity_Mode_Increases_The_Loan_Amount_With_The_Term()
    {
        const decimal maximumInstallment = 1_000_000m;
        var alternatives = new SimulationEngine(Policy).Calculate(new SimulationInput(SimulationMode.InstallmentCapacity, 10_000_000m, 1_000_000m, null, maximumInstallment));

        Assert.AreEqual(5, alternatives.Count);
        Assert.IsTrue(alternatives.All(x => x.AmountCop >= Policy.MinimumAmountCop));
        Assert.IsTrue(alternatives.Zip(alternatives.Skip(1)).All(pair => pair.First.AmountCop < pair.Second.AmountCop));
        Assert.IsTrue(alternatives.All(x => x.MonthlyInstallmentCop == maximumInstallment));
    }
}
