using CuotaClara.Domain.Simulation;

namespace CuotaClara.Application.Abstractions;

public sealed record Activity(string Id, string Name);
public sealed record Agreement(string Id, string ActivityId, string Name);
public interface ICatalogRepository
{
    IReadOnlyList<Activity> GetActivities();
    IReadOnlyList<Agreement> GetAgreements(string activityId);
    bool ActivityExists(string activityId);
    bool AgreementIsCompatible(string activityId, string agreementId);
}
public interface ICreditPolicyProvider { CreditPolicy Get(); }
