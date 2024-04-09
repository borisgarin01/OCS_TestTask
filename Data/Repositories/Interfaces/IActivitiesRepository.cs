using OCS_TestTask.Models.DTOs;

namespace OCS_TestTask.Repositories.Interfaces
{
    public interface IActivitiesRepository
    {
        Task<IEnumerable<Activity>> GetAvailableActivitiesTypes();
    }
}
