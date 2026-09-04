using CuotaClara.Application.Abstractions;

namespace CuotaClara.Application.Catalogs;

public sealed class CatalogService(ICatalogRepository repository)
{
    public IReadOnlyList<Activity> GetActivities() => repository.GetActivities();
    public IReadOnlyList<Agreement> GetAgreements(string activityId)
    {
        if (string.IsNullOrWhiteSpace(activityId) || !repository.ActivityExists(activityId)) throw new ApplicationValidationException("Activity was not found.");
        return repository.GetAgreements(activityId);
    }
}

public sealed class ApplicationValidationException(string message) : Exception(message);
