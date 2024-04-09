using Data.Migrations;
using FluentMigrator.Runner;
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

            var serviceProvider = new ServiceCollection()
     // Logging is the replacement for the old IAnnouncer
     .AddLogging(lb => lb.AddFluentMigratorConsole())
     // Registration of all FluentMigrator-specific services
     .AddFluentMigratorCore()
     // Configure the runner
     .ConfigureRunner(
         b => b
             // Use SQLite
             .AddPostgres()
             .ConfigureGlobalProcessorOptions(opt =>
             {
                 opt.ProviderSwitches = "Force Quote=false";
             })
             // The SQLite connection string
             .WithGlobalConnectionString(connectionString)
             // Specify the assembly with the migrations
             .WithMigrationsIn(typeof(InitialMigration).Assembly))
     .BuildServiceProvider();

            // Put the database update into a scope to ensure
            // that all resources will be disposed.
            using (var scope = serviceProvider.CreateScope())
            {
                // Instantiate the runner
                var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                runner.ListMigrations();
                // Execute the migrations
                runner.MigrateUp();
            }
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
