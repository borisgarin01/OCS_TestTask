using OCS_TestTask.Models.DTOs;
using OCS_TestTask.Models.Models;

namespace OCS_TestTask.Repositories.Interfaces
{
    public interface IApplicationsRepository
    {
        public Task<IEnumerable<Application>> GetAllApplicationsAsync();
        public Task DeleteApplicationAsync(Guid id);
        public Task AddApplicationAsync(Application application);
        public Task UpdateApplicationAsync(Application application, ApplicationUpdatingPart applicationUpdatingPart);
        public Task SendToComitteeConsiderationAsync(Guid applicationId);
        public Task<int> GetAuthorSubmittedApplicationsCount(Guid authorId);
        public Task<int> GetAuthorAllApplicationsCount(Guid authorId);
        public Task<IEnumerable<Application>> GetApplicationsSubmittedAfterDateAsync(DateTime submittedAfter);
        public Task<IEnumerable<Application>> GetApplicationsUnsubmittedOlderDateAsync(DateTime unsubmittedOlder);
        public Task<Application> GetApplicationByIdAsync(Guid applicationId);
    }
}
