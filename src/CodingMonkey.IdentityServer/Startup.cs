﻿namespace CodingMonkey.IdentityServer
{
    using System;
    using System.IO;
    using System.Security.Cryptography.X509Certificates;

    using CodingMonkey.IdentityServer.Models;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using Serilog;
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
                .AddJsonFile("appsettings.secrets.json", optional: false)
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
            else
            {
                Log.Logger = new LoggerConfiguration()
                                    .WriteTo.ApplicationInsightsEvents(Configuration["ApplicationInsights:InstrumentationKey"])
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
            var cert = this.LoadIdentityServerSignCert();
            if (cert != null)
            {
                builder.SetSigningCredential(cert);
            }
            else
            {
                builder.SetTemporarySigningCredential();
            }

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

        private X509Certificate2 LoadIdentityServerSignCert()
        {
            if (env.IsDevelopment())
            {
                string identityServerCertPassword = Configuration["IdentityServer:Password"];
                string identityServerCertFileName = Configuration["IdentityServer:SignCert:Filename"];
                return new X509Certificate2(Path.Combine(this.env.ContentRootPath, identityServerCertFileName), identityServerCertPassword);
            }

            string identityServerCertAzureThumbprint = Configuration["IdentityServer:SignCert:AzureThumbprint"];
            X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            certStore.Open(OpenFlags.ReadOnly);

            X509Certificate2Collection certCollection = new X509Certificate2Collection();

            try
            {
                certCollection = certStore
                                    .Certificates
                                    .Find(X509FindType.FindByThumbprint,
                                        identityServerCertAzureThumbprint, // Generated by Azure
                                        false);
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, "There was an error creating the cert. Did you set the WEBSITE_LOAD_CERTIFICATES app setting?");
                throw new FileNotFoundException(
                    "The certificate was not loaded in the personal cert store. Did you set the WEBSITE_LOAD_CERTIFICATES app setting?",
                    e);
            }

            if (certCollection.Count > 0)
            {
                X509Certificate2 cert = certCollection[0];
                return cert;
            }
            certStore.Dispose();

            return null;
        }
    }
}
