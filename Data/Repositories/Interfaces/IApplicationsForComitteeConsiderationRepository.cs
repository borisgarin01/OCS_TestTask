using OCS_TestTask.Models.DTOs;

namespace OCS_TestTask.Repositories.Interfaces
{
    public interface IApplicationsForComitteeConsiderationRepository
    {
        Task<ApplicationForComitteeConsideration> GetApplicationForComitteeConsideration(Guid id);
    }
}
