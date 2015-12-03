using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Hosting;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using WcsVideos.Controllers;
using WcsVideos.Providers;

namespace WcsVideos
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {           
            // Setup configuration sources.
            var configuration = new Configuration()
                .AddJsonFile("config.json")
                .AddJsonFile($"config.{env.EnvironmentName}.json", optional: true);

            if (env.IsEnvironment("Development"))
            {
                // This reads the configuration keys from the secret store.
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                configuration.AddUserSecrets();
            }
            
            configuration.AddEnvironmentVariables();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add Application settings to the services container.
            services.Configure<AppSettings>(Configuration.GetSubKey("AppSettings"));

            YoutubeVideoDetailsProvider.GoogleApiKey = this.Configuration.GetSubKey("AppSettings")["GoogleApiKey"];

            // Add MVC services to the services container.
            services.AddMvc();

            string key = Guid.NewGuid().ToString("N");
            services.AddSingleton(typeof(Contracts.IDataAccess), typeof(Contracts.MockDataAccess));
            // services.AddSingleton(
            //     typeof(IDataAccess),
            //     (serviceProvider) => new CachingDataAccess(
            //         this.Configuration.GetSubKey("AppSettings")["DataAccessEndpoint"]));
                    
            services.AddSingleton(
                typeof(IUserSessionHandler),
                (serviceProvider) => new UserSessionHandler(
                    this.Configuration.GetSubKey("AppSettings")["Username"],
                    this.Configuration.GetSubKey("AppSettings")["Password"],
                    key));
        }

        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerfactory)
        {
            // Configure the HTTP request pipeline.

            // Add the console logger.
            loggerfactory.AddConsole(minLevel: LogLevel.Warning);

            // Add the following to the request pipeline only in development environment.
            if (env.IsEnvironment("Development"))
            {
                app.UseErrorPage(ErrorPageOptions.ShowAll);
                // app.UseDatabaseErrorPage(DatabaseErrorPageOptions.ShowAll);
            }
            else
            {
                // Add Error handling middleware which catches all application specific errors and
                // sends the request to the following path or controller action.
                app.UseErrorHandler("/Home/Error");
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
