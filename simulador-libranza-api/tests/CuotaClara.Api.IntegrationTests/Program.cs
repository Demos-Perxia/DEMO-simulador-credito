using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CuotaClara.Api.IntegrationTests;

[TestClass]
public sealed class CatalogAndSimulationEndpointsTests
{
    private readonly WebApplicationFactory<Program> _factory = new();

    [TestMethod]
    public async Task Activities_Returns_Mock_Catalog()
    {
        using var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/v1/catalogs/activities");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        StringAssert.Contains(body, "Docente");
        StringAssert.Contains(body, "Pensionado");
    }

    [TestMethod]
    public async Task Incompatible_Agreement_Returns_Problem_Details()
    {
        using var client = _factory.CreateClient();
        var payload = new { activityId = "teacher", agreementId = "pensioner-national", mode = "AMOUNT", monthlyIncomeCop = 10_000_000m, payrollDeductionsCop = 1_000_000m, requestedAmountCop = 20_000_000m };
        var response = await client.PostAsJsonAsync("/api/v1/credit-simulations", payload);
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.AreEqual("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [TestMethod]
    public async Task Simulation_Returns_All_Alternatives()
    {
        using var client = _factory.CreateClient();
        var payload = new { activityId = "teacher", agreementId = "teacher-public", mode = "AMOUNT", monthlyIncomeCop = 10_000_000m, payrollDeductionsCop = 1_000_000m, requestedAmountCop = 20_000_000m };
        var response = await client.PostAsJsonAsync("/api/v1/credit-simulations", payload);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(body);
        var alternatives = document.RootElement.GetProperty("alternatives");
        Assert.AreEqual(5, alternatives.GetArrayLength());
        CollectionAssert.AreEqual(new[] { 60, 72, 96, 108, 120 }, alternatives.EnumerateArray().Select(item => item.GetProperty("termInMonths").GetInt32()).ToArray());
    }
}
