using Dapper;
using Npgsql;
using OCS_TestTask.Models.Models;
using OCS_TestTask.Repositories.Interfaces;

namespace OCS_TestTask.Repositories.Classes
{
    public sealed class ApplicationsRepository : IApplicationsRepository
    {
        private readonly string _connectionString;

        public ApplicationsRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task AddApplicationAsync(Application application)
        {
            using (NpgsqlConnection npgsqlConnection = new NpgsqlConnection(_connectionString))
            {
                await npgsqlConnection.ExecuteAsync("INSERT INTO Applications (Id,AuthorId,Activity,Name,Description,Outline,CreationTimeStamp) VALUES(@Id,@AuthorId,@Activity,@Name,@Description,@Outline,@CreationTimeStamp)", new { Id = Guid.NewGuid(), application.AuthorId, application.Activity, application.Name, application.Description, application.Outline, application.CreationTimeStamp });
            }
        }

        public async Task DeleteApplicationAsync(Guid id)
        {
            using (NpgsqlConnection npgsqlConnection = new NpgsqlConnection(_connectionString))
            {
                await npgsqlConnection.ExecuteAsync("DELETE FROM Applications WHERE Id=@Id", new { Id = id });
            }
        }

        public async Task<IEnumerable<Application>> GetAllApplicationsAsync()
        {
            IEnumerable<Application> applications;
            using (NpgsqlConnection npgsqlConnection = new NpgsqlConnection(_connectionString))
            {
                applications = await npgsqlConnection.QueryAsync<Application>("SELECT * FROM Applications");
            }
            return applications;
        }

        public async Task<Application> GetApplicationByIdAsync(Guid applicationId)
        {
            Application application;
            using (NpgsqlConnection npgsqlConnection = new NpgsqlConnection(_connectionString))
            {
                application = await npgsqlConnection.QueryFirstOrDefaultAsync<Application>($"SELECT * FROM Applications Where Id=@ApplicationId", new { ApplicationId = applicationId });
            }
            return application;
        }

        public async Task<IEnumerable<Application>> GetApplicationsSubmittedAfterDateAsync(DateTime submittedAfter)
        {
            IEnumerable<Application> applications;
            using (NpgsqlConnection npgsqlConnection = new NpgsqlConnection(_connectionString))
            {
                applications = await npgsqlConnection.QueryAsync<Application>($"SELECT Applications.Id, Applications.AuthorId, Applications.Activity, Applications.Name, Applications.Description, Applications.Outline FROM Applications INNER JOIN applications_for_comittee_consideration on applications_for_comittee_consideration.application_id=Applications.Id WHERE applications_for_comittee_consideration.submitting_timestamp > '{submittedAfter.ToString("yyyy-MM-dd")}'");
            }
            return applications;
        }

        public async Task<IEnumerable<Application>> GetApplicationsUnsubmittedOlderDateAsync(DateTime unsubmittedOlder)
        {
            IEnumerable<Application> applications;
            using (NpgsqlConnection npgsqlConnection = new NpgsqlConnection(_connectionString))
            {
                applications = await npgsqlConnection.QueryAsync<Application>($"SELECT Applications.Id, Applications.AuthorId, Applications.Activity, Applications.Name, Applications.Description, Applications.Outline FROM Applications LEFT JOIN applications_for_comittee_consideration on Applications.Id = applications_for_comittee_consideration.application_id WHERE applications_for_comittee_consideration.application_id is null and Applications.creationtimestamp > '{unsubmittedOlder.ToString("yyyy-MM-dd")}'");
            }
            return applications;
        }

        public async Task<int> GetAuthorAllApplicationsCount(Guid authorId)
        {
            int authorApplicationCount;

            using (NpgsqlConnection npgsqlConnection = new NpgsqlConnection(_connectionString))
            {
                authorApplicationCount = await npgsqlConnection.QueryFirstAsync<int>("SELECT COUNT(*) FROM Applications WHERE AuthorId=@AuthorId", new { AuthorId = authorId });
            }
            return authorApplicationCount;
        }

        public async Task<int> GetAuthorSubmittedApplicationsCount(Guid authorId)
        {
            using (NpgsqlConnection npgsqlConnection = new NpgsqlConnection(_connectionString))
            {
                int authorApplicationsForComitteeConsiderationCount = await npgsqlConnection.QueryFirstAsync<int>("SELECT COUNT(*) FROM Applications INNER JOIN applications_for_comittee_consideration on Applications.Id=applications_for_comittee_consideration.application_id WHERE AuthorId=@AuthorId", new { AuthorId = authorId });
                return authorApplicationsForComitteeConsiderationCount;
            }
        }
        public async Task SendToComitteeConsiderationAsync(Guid applicationId)
        {
            using (NpgsqlConnection npgsqlConnection = new NpgsqlConnection(_connectionString))
            {
                await npgsqlConnection.ExecuteAsync("INSERT INTO applications_for_comittee_consideration(id, application_id, submitting_timestamp) VALUES(@Id, @Application_Id, @SubmittingTimeStamp);", new ApplicationForComitteeConsideration { Id = Guid.NewGuid(), Application_Id = applicationId, SubmittingTimeStamp = DateTime.UtcNow });
            }
        }

        public async Task UpdateApplicationAsync(Application application, ApplicationUpdatingPart applicationUpdatingPart)
        {
            using (NpgsqlConnection npgsqlConnection = new NpgsqlConnection(_connectionString))
            {
                await npgsqlConnection.ExecuteAsync("UPDATE Applications SET Outline=@Outline, Activity=@Activity, Description=@Description, Name=@Name", new { applicationUpdatingPart.Outline, applicationUpdatingPart.Activity, applicationUpdatingPart.Description, applicationUpdatingPart.Name });
            }
        }
    }
}
