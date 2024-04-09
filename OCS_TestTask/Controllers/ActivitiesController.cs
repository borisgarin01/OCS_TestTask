using Microsoft.AspNetCore.Mvc;
using OCS_TestTask.Models.DTOs;
using OCS_TestTask.Repositories.Interfaces;

namespace OCS_TestTask.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class ActivitiesController : ControllerBase
    {
        private readonly IActivitiesRepository _activitiesRepository;

        public IConfiguration Configuration { get; }
        public ActivitiesController(IConfiguration configuration, IActivitiesRepository activitiesRepository)
        {
            Configuration = configuration;
            _activitiesRepository = activitiesRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Activity>>> GetAvailableActivitiesTypes()
        {
            IEnumerable<Activity> activities = await _activitiesRepository.GetAvailableActivitiesTypes();
            return Ok(activities);
        }
    }
}
