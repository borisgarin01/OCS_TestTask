using Microsoft.AspNetCore.Mvc;
using Npgsql;
using OCS_TestTask.Models.Models;
using OCS_TestTask.Repositories.Interfaces;

namespace OCS_TestTask.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class ApplicationsController : ControllerBase
    {
        private readonly IApplicationsRepository _applicationsRepository;
        private readonly IApplicationsForComitteeConsiderationRepository _applicationsForComitteeConsiderationRepository;

        public IConfiguration Configuration { get; }
        public ApplicationsController(IConfiguration configuration, IApplicationsRepository applicationsRepository, IApplicationsForComitteeConsiderationRepository applicationsForComitteeConsiderationRepository)
        {
            Configuration = configuration;
            _applicationsRepository = applicationsRepository;
            _applicationsForComitteeConsiderationRepository = applicationsForComitteeConsiderationRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Application>>> Get()
        {
            IEnumerable<Application> applications = await _applicationsRepository.GetAllApplicationsAsync();
            return Ok(applications);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteApplication(Guid id)
        {
            Application application = await _applicationsRepository.GetApplicationByIdAsync(id);
            if (application is null)
            {
                return NotFound($"Заявка с Id={id} не найдена");
            }
            else
            {
                ApplicationForComitteeConsideration applicationForConsideration = await _applicationsForComitteeConsiderationRepository.GetApplicationForComitteeConsideration(id);
                if (applicationForConsideration is null)
                {
                    _applicationsRepository.DeleteApplicationAsync(id);
                    return Ok("Заявка успешно удалена");
                }
                else return BadRequest("Нельзя удалить заявку, находящуюся на рассмотрении");
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddApplication(Application application)
        {
            try
            {
                int authorApplicationsForComitteeConsiderationCount = await _applicationsRepository.GetAuthorSubmittedApplicationsCount(application.AuthorId);

                int authorApplicationCount = await _applicationsRepository.GetAuthorAllApplicationsCount(application.AuthorId);

                if (authorApplicationsForComitteeConsiderationCount == authorApplicationCount)
                {
                    await _applicationsRepository.AddApplicationAsync(application);
                    return Ok();
                }
                return BadRequest("У вас есть неподанная заявка в черновиках");
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPut]
        public async Task<ActionResult<Application>> UpdateApplication(Guid id, ApplicationUpdatingPart applicationUpdatingPart)
        {
            Application application = await _applicationsRepository.GetApplicationByIdAsync(id);
            if (application is null)
            {
                return NotFound();
            }
            else
            {
                try
                {
                    await _applicationsRepository.UpdateApplicationAsync(application, applicationUpdatingPart);

                    Application updatedApplication = await _applicationsRepository.GetApplicationByIdAsync(id);

                    return Ok(updatedApplication);
                }
                catch
                {
                    return BadRequest();
                }
            }
        }

        [HttpPost("{applicationId}")]
        public async Task<ActionResult> SendToComitteeConsideration(Guid applicationId)
        {
            ApplicationForComitteeConsideration applicationForConsideration = await _applicationsForComitteeConsiderationRepository.GetApplicationForComitteeConsideration(applicationId);

            if (applicationForConsideration is null)
            {
                await _applicationsRepository.SendToComitteeConsiderationAsync(applicationId);
                return Ok();
            }
            return BadRequest();
        }

        [HttpGet("submittedAfter={submittedAfter}")]
        public async Task<ActionResult<IEnumerable<Application>>> GetApplicationsSubmittedAfterDate(DateTime submittedAfter)
        {
            IEnumerable<Application> applications = await _applicationsRepository.GetApplicationsSubmittedAfterDateAsync(submittedAfter);
            if (applications == null)
            {
                return NotFound();
            }
            return Ok(applications);
        }

        [HttpGet("unsubmittedOlder={unsubmittedOlder}")]
        public async Task<ActionResult<IEnumerable<Application>>> GetApplicationsUnsubmittedOlderDate(DateTime unsubmittedOlder)
        {
            IEnumerable<Application> applications = await _applicationsRepository.GetApplicationsUnsubmittedOlderDateAsync(unsubmittedOlder);

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
                application = await _applicationsRepository.GetApplicationByIdAsync(applicationId);
            }
            if (application == null)
            {
                return NotFound();
            }
            return Ok(application);
        }
    }
}
