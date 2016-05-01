using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WcsVideos.Contracts;
using WcsVideos.Controllers;
using WcsVideos.Providers;

namespace WcsVideos
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {           
            // Setup configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("config.json")
                .AddJsonFile($"config.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add Application settings to the services container.
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            YoutubeVideoDetailsProvider.GoogleApiKey = this.Configuration.GetSection("AppSettings")["GoogleApiKey"];

            // Add MVC services to the services container.
            services.AddMvc();

            string key = Guid.NewGuid().ToString("N");
            // services.AddSingleton(typeof(Contracts.IDataAccess), typeof(Contracts.MockDataAccess));
            services.AddSingleton(
                typeof(IDataAccess),
                (serviceProvider) => new CachingDataAccess(
                    this.Configuration.GetSection("AppSettings")["DataAccessEndpoint"]));
                    
            services.AddSingleton(
                typeof(IUserSessionHandler),
                (serviceProvider) => new UserSessionHandler(
                    this.Configuration.GetSection("AppSettings")["Username"],
                    this.Configuration.GetSection("AppSettings")["Password"],
                    key));
        }

        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerfactory)
        {
            // Configure the HTTP request pipeline.

            // Add the console logger.
            loggerfactory.AddConsole(minLevel: LogLevel.Warning);

            // Add the following to the request pipeline only in development environment.
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // app.UseDatabaseErrorPage(DatabaseErrorPageOptions.ShowAll);
            }
            else
            {
                // Add Error handling middleware which catches all application specific errors and
                // sends the request to the following path or controller action.
                app.UseExceptionHandler("/Home/Error");
            }

            // Add static files to the request pipeline.
            app.UseStaticFiles();

            // Add cookie-based authentication to the request pipeline.
            // app.UseIdentity();

            // Add MVC to the request pipeline.
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
