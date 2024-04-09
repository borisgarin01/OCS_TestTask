using Dapper;
using Npgsql;
using OCS_TestTask.Models.Models;
using OCS_TestTask.Repositories.Interfaces;

namespace OCS_TestTask.Repositories.Classes
{
    public sealed class UsersCurrentApplicationsRepository : IUsersCurrentApplicationsRepository
    {
        private readonly string _connectionString;

        public UsersCurrentApplicationsRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<Application> GetCurrentApplicationForUserAsync(Guid userId)
        {
            Application application;
            using (var npgsqlConnection = new NpgsqlConnection(_connectionString))
            {
                application = await npgsqlConnection.QueryFirstOrDefaultAsync<Application>("SELECT * FROM Applications LEFT JOIN applications_for_comittee_consideration on Applications.Id = applications_for_comittee_consideration.application_id WHERE applications_for_comittee_consideration.application_id is null and Applications.AuthorId=@AuthorId;", new { AuthorId = userId });
            }
            return application;
        }
    }
}
