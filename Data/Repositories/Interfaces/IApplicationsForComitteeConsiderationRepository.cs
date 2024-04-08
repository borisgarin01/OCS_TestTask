using OCS_TestTask.Models.Models;

namespace OCS_TestTask.Repositories.Interfaces
{
    public interface IApplicationsForComitteeConsiderationRepository
    {
        Task<ApplicationForComitteeConsideration> GetApplicationForComitteeConsideration(Guid id);
    }
}
