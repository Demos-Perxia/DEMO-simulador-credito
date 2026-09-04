using CuotaClara.Application.Abstractions;

namespace CuotaClara.Infrastructure.Catalogs;

public sealed class InMemoryCatalogRepository : ICatalogRepository
{
    private static readonly Activity[] Activities = [new("teacher", "Docente"), new("pensioner", "Pensionado")];
    private static readonly Agreement[] Agreements = [new("teacher-public", "teacher", "Docentes sector público"), new("teacher-private", "teacher", "Docentes sector privado"), new("pensioner-national", "pensioner", "Pensionados nacionales")];
    public IReadOnlyList<Activity> GetActivities() => Activities;
    public IReadOnlyList<Agreement> GetAgreements(string activityId) => Agreements.Where(x => x.ActivityId == activityId).ToArray();
    public bool ActivityExists(string activityId) => Activities.Any(x => x.Id == activityId);
    public bool AgreementIsCompatible(string activityId, string agreementId) => Agreements.Any(x => x.ActivityId == activityId && x.Id == agreementId);
}
