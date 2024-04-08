using OCS_TestTask.Repositories.Classes;
using OCS_TestTask.Repositories.Interfaces;

namespace OCS_TestTask
{
    public sealed class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddScoped<IActivitiesRepository, ActivitiesRepository>(instance => new ActivitiesRepository(connectionString));
            builder.Services.AddScoped<IApplicationsRepository, ApplicationsRepository>(instance => new ApplicationsRepository(connectionString));
            builder.Services.AddScoped<IActivitiesRepository, ActivitiesRepository>(instance => new ActivitiesRepository(connectionString));
            builder.Services.AddScoped<IUsersCurrentApplicationsRepository, UsersCurrentApplicationsRepository>(instance => new UsersCurrentApplicationsRepository(connectionString));
            builder.Services.AddScoped<IApplicationsForComitteeConsiderationRepository, ApplicationsForComitteeConsiderationRepository>(instance => new ApplicationsForComitteeConsiderationRepository(connectionString));

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
