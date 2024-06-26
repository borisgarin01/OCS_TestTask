﻿using Dapper;
using Npgsql;
using OCS_TestTask.Models.DTOs;
using OCS_TestTask.Repositories.Interfaces;

namespace OCS_TestTask.Repositories.Classes
{
    public sealed class ApplicationsForComitteeConsiderationRepository : IApplicationsForComitteeConsiderationRepository
    {
        private readonly string _connectionString;
        public ApplicationsForComitteeConsiderationRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<ApplicationForComitteeConsideration> GetApplicationForComitteeConsideration(Guid id)
        {
            ApplicationForComitteeConsideration applicationForConsideration;
            using (var npgsqlConnection = new NpgsqlConnection(_connectionString))
            {
                applicationForConsideration = await npgsqlConnection.QueryFirstOrDefaultAsync("SELECT * FROM ApplicationsForComitteeConsideration Where ApplicationId=@id", new { id });
            }
            return applicationForConsideration;
        }
    }
}
