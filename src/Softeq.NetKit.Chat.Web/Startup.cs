// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CorrelationId;
using FluentValidation.AspNetCore;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Softeq.NetKit.Chat.Data.Persistent.Database;
using Softeq.NetKit.Chat.SignalR.Hubs;
using Softeq.NetKit.Chat.Web.App.Versioning;
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
            services.AddMvc();
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
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
                options.ApiVersionReader = new HeaderApiVersionReader("api-version");
            });

            services.AddCors();

            services.AddSignalR();

            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme { In = "header", Description = "Please enter JWT with Bearer into field", Name = "Authorization", Type = "apiKey" });

                c.SwaggerDoc("v1.0", new Info { Title = "API doc v1.0", Version = "v1.0" });

                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
                    { "Bearer", new string[0] }
                });

                c.DocInclusionPredicate((docName, apiDesc) =>
                {
                    var actionApiVersionModel = apiDesc.ActionDescriptor?.GetApiVersion();

                    // would mean this action is unversioned and should be included everywhere
                    if (actionApiVersionModel == null)
                    {
                        return true;
                    }

                    if (actionApiVersionModel.DeclaredApiVersions.Any())
                    {
                        return actionApiVersionModel.DeclaredApiVersions.Any(v => $"v{v.ToString()}" == docName);
                    }

                    return actionApiVersionModel.ImplementedApiVersions.Any(v => $"v{v.ToString()}" == docName);
                });

                c.OperationFilter<ApiVersionOperationFilter>();
            });

            var builder = new ContainerBuilder();
            builder.RegisterSolutionModules();
            builder.AddLogger();
            builder.Populate(services);
            _applicationContainer = builder.Build();
            return new AutofacServiceProvider(_applicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async Task Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IDatabaseManager databaseManager, IApplicationLifetime applicationLifetime, ILogger logger)
        {
            applicationLifetime.ApplicationStopping.Register(OnShutdown, app);

            //To dispose of resources that have been resolved in the application container
            //http://autofaccn.readthedocs.io/en/latest/integration/aspnetcore.html#quick-start-without-configurecontainer
            applicationLifetime.ApplicationStopped.Register(() => _applicationContainer.Dispose());

            try
            {
                if (!env.IsStaging() && !env.IsProduction())
                {
                    await databaseManager.CreateEmptyDatabaseIfNotExistsAsync();
                }
                databaseManager.MigrateToLatestVersion();
            }
            catch (Exception ex)
            {
                logger.Event("UnableToSetupDatabase").With.Message("Unable to seed database.").Exception(ex).AsFatal();
                throw;
            }

            // TODO: add `&& !env.IsStaging()` when testing will be passed
            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();

                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger();
                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint($"/swagger/v1.0/swagger.json", "Versioned Api v1.0");
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
