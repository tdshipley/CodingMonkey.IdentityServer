namespace CodingMonkey.IdentityServer
{
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography.X509Certificates;

    using CodingMonkey.IdentityServer.Extensions;
    using CodingMonkey.IdentityServer.Models;

    using IdentityServer4.Core.Services;
    using IdentityServer4.Core.Services.InMemory;
    using IdentityServer4.Core.Validation;

    using Microsoft.AspNet.Builder;
    using Microsoft.AspNet.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.PlatformAbstractions;

    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var cert = new X509Certificate2(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "idsrv4test.pfx"), "idsrv3test");

            var builder = services.AddIdentityServer(options =>
            {
                options.SigningCertificate = cert;
            });

            var x = Clients.Get();
            var y = Scopes.Get();

            builder.AddInMemoryClients(Clients.Get());
            builder.AddInMemoryScopes(Scopes.Get());
            builder.AddInMemoryUsers(new List<InMemoryUser>());

            // Add framework services.
            services.AddMvc();

            builder.Services.AddTransient<IProfileService, InMemoryProfileService>();
            builder.Services.AddTransient<IResourceOwnerPasswordValidator, DefaultResouceOwnerPasswordValidator>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseIISPlatformHandler();

            app.UseIdentityServer();

            app.UseStaticFiles();

            app.UseMvc();
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
