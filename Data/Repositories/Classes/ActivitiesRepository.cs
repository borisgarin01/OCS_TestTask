using Dapper;
using Npgsql;
using OCS_TestTask.Models.Models;
using OCS_TestTask.Repositories.Interfaces;

namespace OCS_TestTask.Repositories.Classes
{
    public sealed class ActivitiesRepository : IActivitiesRepository
    {
        private readonly string _connectionString;

        public ActivitiesRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Activity>> GetAvailableActivitiesTypes()
        {
            IEnumerable<Activity> activities;
            using (NpgsqlConnection npgsqlConnection = new NpgsqlConnection(_connectionString))
            {
                activities = await npgsqlConnection.QueryAsync<Activity>("SELECT * FROM Activities");
            }
            return activities;
        }
    }
}
