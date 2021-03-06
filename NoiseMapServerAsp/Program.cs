using DAL;
using DAL.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoiseMapServerAsp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args)
                .ConfigureLogging((hostingContext, builder) =>
                {
                    builder.AddFile(hostingContext.Configuration.GetSection("Logging"));
                })
                .Build();
            try
            {
                SetDefaultSeed(host);
                host.Run();
            }
            catch (Exception ex)
            {
                var scope = host.Services.CreateScope();
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError($"UserDomainName: {Environment.UserDomainName} UserName: {Environment.UserName}");
                logger.LogError(ex, "");
                throw;
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static void SetDefaultSeed(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var applicationContext = services.GetRequiredService<ApplicationContext>();
            applicationContext.Database.EnsureCreated();


            var userManager = services.GetRequiredService<UserManager<User>>();
            var user = new User { UserName = "123", Email = "123" };
            var result = Task.Run(async () => await userManager.CreateAsync(user, "123")).Result;
        }
    }
}
