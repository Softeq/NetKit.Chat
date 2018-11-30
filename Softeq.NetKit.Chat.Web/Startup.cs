// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CorrelationId;
using FluentValidation.AspNetCore;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Softeq.NetKit.Chat.Data.Persistent.Database;
using Softeq.NetKit.Chat.SignalR.Hubs;
using Softeq.NetKit.Chat.Web.Configuration;
using Softeq.NetKit.Chat.Web.ExceptionHandling;
using Softeq.NetKit.Chat.Web.Extensions;
using Softeq.NetKit.Chat.Web.Filters;
using Softeq.Serilog.Extension;
using Swashbuckle.AspNetCore.Swagger;
using ILogger = Serilog.ILogger;

namespace Softeq.NetKit.Chat.Web
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private IContainer _applicationContainer;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore(options =>
                {
                    options.Filters.Add<ValidateModelStateFilter>();
                })
                .AddFluentValidation(configuration => configuration.RegisterValidatorsFromAssemblyContaining<Startup>())
                .AddApiExplorer()
                .AddAuthorization()
                .AddJsonFormatters();

            var authenticationsConfiguration = new AuthenticationsConfiguration();
            _configuration.GetSection("Authentications").Bind(authenticationsConfiguration);

            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.ApiSecret = authenticationsConfiguration.Bearer.ApiSecret;
                    options.Authority = authenticationsConfiguration.Bearer.Authority;
                    options.RequireHttpsMetadata = authenticationsConfiguration.Bearer.RequireHttpsMetadata;

                    options.ApiName = authenticationsConfiguration.Bearer.ApiName;
                });

            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
            });

            services.AddCors();

            services.AddSignalR();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "API doc", Version = "v1" });
            });

            var builder = new ContainerBuilder();
            builder.RegisterSolutionModules();
            builder.AddLogger();
            builder.Populate(services);
            _applicationContainer = builder.Build();
            return new AutofacServiceProvider(_applicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IDatabaseManager databaseManager, IApplicationLifetime applicationLifetime, ILogger logger)
        {
            applicationLifetime.ApplicationStopping.Register(OnShutdown, app);

            //To dispose of resources that have been resolved in the application container
            //http://autofaccn.readthedocs.io/en/latest/integration/aspnetcore.html#quick-start-without-configurecontainer
            applicationLifetime.ApplicationStopped.Register(() => _applicationContainer.Dispose());

            try
            {
                if (!env.IsStaging() && !env.IsProduction())
                {
                    databaseManager.CreateEmptyDatabaseIfNotExistsAsync().GetAwaiter().GetResult();
                }
                databaseManager.MigrateToLatestVersion();
            }
            catch (Exception ex)
            {
                logger.Event("UnableToSetupDatabase").With.Message("Unable to seed database.").Exception(ex).AsFatal();
                throw;
            }

            if (!env.IsStaging() && !env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();

                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger();
                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                });
            }

            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseCorrelationId();
            app.UseAuthentication();
            app.UseSignalR(routes =>
            {
                routes.MapHub<ChatHub>("/chat");
            });
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseMvc();
        }

        private void OnShutdown(object builder)
        {
            if (builder is IApplicationBuilder applicationBuilder)
            {
                var telemetryClient = applicationBuilder.ApplicationServices.GetService<TelemetryClient>();
                if (telemetryClient != null)
                {
                    telemetryClient.Flush();
                    //Wait while the data is flushed
                    System.Threading.Thread.Sleep(1000);
                }
                Log.CloseAndFlush();
            }
        }
    }
}
