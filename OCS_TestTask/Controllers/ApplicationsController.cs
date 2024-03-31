using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Npgsql;
using System.ComponentModel.DataAnnotations;
using OCS_TestTask.Models;
using Npgsql.Internal;

namespace OCS_TestTask.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApplicationsController : ControllerBase
    {
        public IConfiguration Configuration { get; }
        public ApplicationsController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Application>>> Get()
        {
            IEnumerable<Application> applications;
            using (NpgsqlConnection npgsqlConnection = new NpgsqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                applications = await npgsqlConnection.QueryAsync<Application>("SELECT * FROM Applications");
            }
            return Ok(applications);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteApplication(Guid id)
        {
            using (NpgsqlConnection npgsqlConnection = new NpgsqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                Application application = await npgsqlConnection.QueryFirstOrDefaultAsync<Application>("SELECT * FROM Applications WHERE Id=@Id", new { Id = id });
                if (application is null)
                {
                    return NotFound($"Заявка с Id={id} не найдена");
                }
                else
                {
                    dynamic applicationForConsideration = await npgsqlConnection.QueryFirstOrDefaultAsync("SELECT * FROM applications_for_comittee_consideration Where application_id=@id", new { id });
                    if (applicationForConsideration is null)
                    {
                        await npgsqlConnection.ExecuteAsync("DELETE FROM Applications WHERE Id=@Id", new { Id = id });
                        return Ok("Заявка успешно удалена");
                    }
                    else return BadRequest("Нельзя удалить заявку, находящуюся на рассмотрении");
                }
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddApplication(Application application)
        {
            using (NpgsqlConnection npgsqlConnection = new NpgsqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                try
                {
                    int authorApplicationsForComitteeConsiderationCount = await npgsqlConnection.QueryFirstAsync<int>("SELECT COUNT(*) FROM Applications INNER JOIN applications_for_comittee_consideration on Applications.Id=applications_for_comittee_consideration.application_id WHERE AuthorId=@AuthorId", new { AuthorId = application.AuthorId });
                    int authorApplicationCount = await npgsqlConnection.QueryFirstAsync<int>("SELECT COUNT(*) FROM Applications WHERE AuthorId=@AuthorId", new { AuthorId = application.AuthorId });
                    if (authorApplicationsForComitteeConsiderationCount == authorApplicationCount)
                    {
                        application.CreationTimeStamp = DateTime.Now;
                        await npgsqlConnection.ExecuteAsync("INSERT INTO Applications (Id,AuthorId,Activity,Name,Description,Outline,CreationTimeStamp) VALUES(@Id,@AuthorId,@Activity,@Name,@Description,@Outline,@CreationTimeStamp)", new { Id = Guid.NewGuid(), application.AuthorId, application.Activity, application.Name, application.Description, application.Outline, application.CreationTimeStamp });
                        return Ok();
                    }
                    return BadRequest("У вас есть неподанная заявка в черновиках");
                }
                catch (Exception ex)
                {
                    return BadRequest();
                }
            }
        }

        [HttpPut]
        public async Task<ActionResult<Application>> UpdateApplication(Guid id, ApplicationUpdatingPart applicationUpdatingPart)
        {
            using (NpgsqlConnection npgsqlConnection = new NpgsqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                Application application = await npgsqlConnection.QueryFirstOrDefaultAsync<Application>("SELECT * FROM Applications WHERE Id=@Id", new { Id = id });
                if (application is null)
                {
                    return NotFound();
                }
                else
                {
                    try
                    {
                        await npgsqlConnection.ExecuteAsync("UPDATE Applications SET Outline=@Outline, Activity=@Activity, Description=@Description, Name=@Name Where Id=@Id", new { applicationUpdatingPart.Outline, applicationUpdatingPart.Activity, applicationUpdatingPart.Description, applicationUpdatingPart.Name, id });
                        return Ok(await npgsqlConnection.QueryFirstOrDefaultAsync<Application>("SELECT * FROM Applications WHERE Id=@Id", new { Id = id }));
                    }
                    catch
                    {
                        return BadRequest();
                    }
                }
            }
        }

        [HttpPost("{applicationId}")]
        public async Task<ActionResult> SendToComitteeConsideation(Guid applicationId)
        {
            using (NpgsqlConnection npgsqlConnection = new NpgsqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                ApplicationForComitteeConsideration applicationForConsideration = await npgsqlConnection.QueryFirstOrDefaultAsync<ApplicationForComitteeConsideration>("SELECT * FROM applications_for_comittee_consideration Where Application_Id=@applicationId", new { applicationId });
                if (applicationForConsideration is null)
                {
                    await npgsqlConnection.ExecuteAsync("INSERT INTO applications_for_comittee_consideration(id, application_id, submitting_timestamp) VALUES(@Id, @Application_Id, @SubmittingTimeStamp);", new ApplicationForComitteeConsideration { Id = Guid.NewGuid(), Application_Id = applicationId, SubmittingTimeStamp = DateTime.UtcNow });
                    return Ok();
                }
                return BadRequest();
            }
        }

        [HttpGet("submittedAfter={submittedAfter}")]
        public async Task<ActionResult<IEnumerable<Application>>> GetApplicationsSubmittedAfterDate(DateTime submittedAfter)
        {
            IEnumerable<Application> applications;
            using (NpgsqlConnection npgsqlConnection = new NpgsqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {

                applications = await npgsqlConnection.QueryAsync<Application>($"SELECT Applications.Id, Applications.AuthorId, Applications.Activity, Applications.Name, Applications.Description, Applications.Outline FROM Applications INNER JOIN applications_for_comittee_consideration on applications_for_comittee_consideration.application_id=Applications.Id WHERE applications_for_comittee_consideration.submitting_timestamp > '{submittedAfter.ToString("yyyy-MM-dd")}'");
            }
            if (applications == null)
            {
                return NotFound();
            }
            return Ok(applications);
        }

        [HttpGet("unsubmittedOlder={unsubmittedOlder}")]
        public async Task<ActionResult<IEnumerable<Application>>> GetApplicationsUnsubmittedOlderDate(DateTime unsubmittedOlder)
        {
            IEnumerable<Application> applications;
            using (NpgsqlConnection npgsqlConnection = new NpgsqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                applications = await npgsqlConnection.QueryAsync<Application>($"SELECT Applications.Id, Applications.AuthorId, Applications.Activity, Applications.Name, Applications.Description, Applications.Outline FROM Applications LEFT JOIN applications_for_comittee_consideration on Applications.Id = applications_for_comittee_consideration.application_id WHERE applications_for_comittee_consideration.application_id is null and Applications.creationtimestamp > '{unsubmittedOlder.ToString("yyyy-MM-dd")}'");
            }
            if (applications == null)
            {
                return NotFound();
            }
            return Ok(applications);
        }

        [HttpGet("{applicationId}")]
        public async Task<ActionResult<Application>> GetApplicationById(Guid applicationId)
        {
            Application application;
            using (NpgsqlConnection npgsqlConnection = new NpgsqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                application = await npgsqlConnection.QueryFirstOrDefaultAsync<Application>($"SELECT * FROM Applications Where Id=@ApplicationId", new { ApplicationId = applicationId });
            }
            if (application == null)
            {
                return NotFound();
            }
            return Ok(application);
        }
    }
}
