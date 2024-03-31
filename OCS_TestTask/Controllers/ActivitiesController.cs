using Dapper;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using OCS_TestTask.Models;

namespace OCS_TestTask.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ActivitiesController:ControllerBase
    {
        public IConfiguration Configuration { get; }
        public ActivitiesController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Activity>>> GetAvailableActivitiesTypes()
        {
            IEnumerable<Activity> activities;
            using (NpgsqlConnection npgsqlConnection = new NpgsqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                activities = await npgsqlConnection.QueryAsync<Activity>("SELECT * FROM Activities");
            }
            return Ok(activities);
        }
    }
}
