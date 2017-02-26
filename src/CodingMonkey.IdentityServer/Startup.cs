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
    using Newtonsoft.Json.Serialization;

    public class Startup
    {
        private IHostingEnvironment env;

        public Startup(IHostingEnvironment env)
        {
            this.env = env;
            string applicationPath = env.ContentRootPath;

            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(applicationPath)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            if (env.IsDevelopment() || env.IsStaging())
            {
                // Create SeriLog
                Log.Logger = new LoggerConfiguration()
                                    .MinimumLevel.Debug()
                                    .WriteTo.RollingFile(Path.Combine(applicationPath, "log_{Date}.txt"))
                                    .CreateLogger();
            }
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //IServ4 Docs: https://github.com/IdentityServer/IdentityServer4/tree/dev/docs
            //TODO: Add certficate for HTTPS!
            var builder = services.AddIdentityServer(options =>
            {
            });
            
            builder.AddInMemoryClients(Clients.Get(this.env.ContentRootPath));
            builder.AddInMemoryScopes(Scopes.Get(this.env.ContentRootPath));
            builder.AddInMemoryStores();

            // Add framework services.
            // Change JSON serialisation to use property names!
            // See: https://weblog.west-wind.com/posts/2016/Jun/27/Upgrading-to-ASPNET-Core-RTM-from-RC2
            services.AddMvc()
                    .AddJsonOptions(opt =>
                    {
                        var resolver = opt.SerializerSettings.ContractResolver;

                        if (resolver != null)
                        {
                            var res = resolver as DefaultContractResolver;
                            res.NamingStrategy = null; // This removes camel casing
                        }
                    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddSerilog();

            app.UseIdentityServer();
            loggerFactory.AddConsole(LogLevel.Debug);

            app.UseStaticFiles();

            app.UseMvc();
        }
    }
}
