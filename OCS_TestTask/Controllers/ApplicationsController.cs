using Microsoft.AspNetCore.Mvc;
using Npgsql;
using OCS_TestTask.Models.DTOs;
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
            var applications = await _applicationsRepository.GetAllApplicationsAsync();
            return Ok(applications);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteApplication(Guid id)
        {
            var application = await _applicationsRepository.GetApplicationByIdAsync(id);
            if (application is null)
            {
                return NotFound($"Заявка с Id={id} не найдена");
            }
            else
            {
                var applicationForConsideration = await _applicationsForComitteeConsiderationRepository.GetApplicationForComitteeConsideration(id);
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
                var authorApplicationsForComitteeConsiderationCount = await _applicationsRepository.GetAuthorSubmittedApplicationsCount(application.AuthorId);

                var authorApplicationCount = await _applicationsRepository.GetAuthorAllApplicationsCount(application.AuthorId);

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
            var application = await _applicationsRepository.GetApplicationByIdAsync(id);
            if (application is null)
            {
                return NotFound();
            }
            else
            {
                try
                {
                    await _applicationsRepository.UpdateApplicationAsync(application, applicationUpdatingPart);
                    var updatedApplication = await _applicationsRepository.GetApplicationByIdAsync(id);

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
            var applicationForConsideration = await _applicationsForComitteeConsiderationRepository.GetApplicationForComitteeConsideration(applicationId);

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
            var applications = await _applicationsRepository.GetApplicationsSubmittedAfterDateAsync(submittedAfter);
            if (applications == null)
            {
                return NotFound();
            }
            return Ok(applications);
        }

        [HttpGet("unsubmittedOlder={unsubmittedOlder}")]
        public async Task<ActionResult<IEnumerable<Application>>> GetApplicationsUnsubmittedOlderDate(DateTime unsubmittedOlder)
        {
            var applications = await _applicationsRepository.GetApplicationsUnsubmittedOlderDateAsync(unsubmittedOlder);

            if (applications == null)
            {
                return NotFound();
            }
            return Ok(applications);
        }

        [HttpGet("{applicationId}")]
        public async Task<ActionResult<Application>> GetApplicationById(Guid applicationId)
        {
            var application = await _applicationsRepository.GetApplicationByIdAsync(applicationId);
            if (application == null)
            {
                return NotFound();
            }
            return Ok(application);
        }
    }
}
