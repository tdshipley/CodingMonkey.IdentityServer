namespace CodingMonkey.IdentityServer
{
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography.X509Certificates;

    using CodingMonkey.IdentityServer.Models;

    using IdentityServer4.Services;
    using IdentityServer4.Services.InMemory;
    using IdentityServer4.Validation;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using Serilog;
    using Serilog.Sinks.RollingFile;

    public class Startup
    {
        private IHostingEnvironment env;

        public Startup(IHostingEnvironment env)
        {
            this.env = env;
            string applicationPath = env.ContentRootPath;

            // Create SeriLog
            Log.Logger = new LoggerConfiguration()
                                .MinimumLevel.Debug()
                                .WriteTo.RollingFile(Path.Combine(applicationPath, "log_{Date}.txt"))
                                .CreateLogger();

            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(applicationPath)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //var cert = new X509Certificate2(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "idsrv4test.pfx"), "idsrv3test");

            var builder = services.AddIdentityServer(options =>
            {
                //options.SigningCertificate = cert;
            });

            builder.AddInMemoryClients(Clients.Get(this.env.ContentRootPath));
            builder.AddInMemoryScopes(Scopes.Get(this.env.ContentRootPath));
            builder.AddInMemoryUsers(new List<InMemoryUser>());

            // Add framework services.
            services.AddMvc();

            builder.Services.AddTransient<IProfileService, InMemoryUserProfileService>();
            builder.Services.AddTransient<IResourceOwnerPasswordValidator, ResouceOwnerPasswordValidator>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddSerilog();

            app.UseIdentityServer();

            app.UseStaticFiles();

            app.UseMvc();
        }
    }
}
