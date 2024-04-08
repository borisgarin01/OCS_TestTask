using OCS_TestTask.Models.Models;

namespace OCS_TestTask.Repositories.Interfaces
{
    public interface IActivitiesRepository
    {
        Task<IEnumerable<Activity>> GetAvailableActivitiesTypes();
    }
}
