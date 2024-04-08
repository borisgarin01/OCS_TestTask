using Microsoft.AspNetCore.Mvc;
using Npgsql;
using OCS_TestTask.Models.Models;
using OCS_TestTask.Repositories.Interfaces;

namespace OCS_TestTask.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class UsersController : ControllerBase
    {
        private readonly IUsersCurrentApplicationsRepository _usersCurrentApplicationsRepository;

        public IConfiguration Configuration { get; }
        public UsersController(IConfiguration configuration, IUsersCurrentApplicationsRepository usersCurrentApplicationsRepository)
        {
            Configuration = configuration;
            _usersCurrentApplicationsRepository = usersCurrentApplicationsRepository;
        }

        [HttpGet("{userId}/currentapplication")]
        public async Task<ActionResult<Application>> GetCurrentApplicationForUserAsync(Guid userId)
        {
            Application application;
            using (NpgsqlConnection npgsqlConnection = new NpgsqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                application = await _usersCurrentApplicationsRepository.GetCurrentApplicationForUserAsync(userId);
            }
            if (application is null)
            {
                return NotFound();
            }
            return Ok(application);
        }
    }
}
