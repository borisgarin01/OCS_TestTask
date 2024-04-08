using OCS_TestTask.Models.Models;

namespace OCS_TestTask.Repositories.Interfaces
{
    public interface IUsersCurrentApplicationsRepository
    {
        public Task<Application> GetCurrentApplicationForUserAsync(Guid userId);
    }
}
