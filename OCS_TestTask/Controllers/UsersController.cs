using Dapper;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using OCS_TestTask.Models;

namespace OCS_TestTask.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        public IConfiguration Configuration { get; }
        public UsersController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [HttpGet("{userId}/currentapplication")]
        public async Task<ActionResult<Application>> GetCurrentApplicationForUserAsync(Guid userId)
        {
            Application application;
            using (NpgsqlConnection npgsqlConnection = new NpgsqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                application = await npgsqlConnection.QueryFirstOrDefaultAsync<Application>("SELECT * FROM Applications LEFT JOIN applications_for_comittee_consideration on Applications.Id = applications_for_comittee_consideration.application_id WHERE applications_for_comittee_consideration.application_id is null and Applications.AuthorId=@AuthorId;", new { AuthorId = userId });
            }
            if (application is null)
            {
                return NotFound();
            }
            return Ok(application);
        }
    }
}
