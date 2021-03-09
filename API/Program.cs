using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            // we need to get our DataContext service so we can pass it to our seed method
            // creating a scope for the services we're going to create 
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            // we write a try - catch block because we don't have access to the error handling middleware f.e.
            try 
            {
                var context = services.GetRequiredService<DataContext>();

                // get our database and migrate it here
                // What it does; so far we've been using dotnet-ef database update to apply migrations.
                // What we do from now on is just restart our application to apply any migration
                // If you drop the database and restart the app, the database will be recreated

                var userManager = services.GetRequiredService<UserManager<AppUser>>();
                var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
                await context.Database.MigrateAsync();

                // seed the database with the Seed method
                await Seed.SeedUsers(userManager, roleManager);
            }
            catch(Exception ex)
            {
                // if we have an error, we log it 
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occured during migration");
            }

            // we removed the run method from the host at the top, so run this here now is very important :)
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
