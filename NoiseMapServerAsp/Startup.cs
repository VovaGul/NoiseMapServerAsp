using BAL;
using BAL.Models;
using DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NoiseMapServerAsp.Controllers;
using NoiseMapServerAsp.Hubs;

namespace NoiseMapServerAsp
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEntityFrameworkSqlite().AddDbContext<ApplicationContext>();

            services.AddSingleton<AudioRepository>();
            services.AddControllers();
            services.AddSingleton(_ =>
            {
                return new Square
                {
                    StartCoordinate = new Coordinate
                    {
                        X = 60.541696,
                        Y = 56.871670
                    },
                    EndCoordinate = new Coordinate 
                    {
                        X = 60.640916,
                        Y = 56.817108
                    },
                };
            });
            services.AddScoped<MarkersSeed>();

            services.AddSignalR();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapHub<ClientConnectionHub>("/update");
            });
        }
    }
}
