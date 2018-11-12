// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CorrelationId;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Softeq.NetKit.Chat.Data.Repositories.Infrastructure;
using Softeq.NetKit.Chat.Infrastructure.SignalR.Hubs;
using Softeq.NetKit.Chat.Web.App;
using Softeq.NetKit.Chat.Web.App.DI;
using Softeq.NetKit.Chat.Web.ExceptionHandling;
using Swashbuckle.AspNetCore.Swagger;

namespace Softeq.NetKit.Chat.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IContainer ApplicationContainer { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore(o =>
                {
                    o.Filters.Add(typeof(GlobalExceptionFilter));
                })
                .AddApiExplorer()
                .AddAuthorization()
                .AddJsonFormatters();

            var authenticationsConfiguration = new AuthenticationsConfiguration();
            Configuration.GetSection("Authentications").Bind(authenticationsConfiguration);

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

            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);
            DIModulesManager.RegisterModules(containerBuilder);

            ApplicationContainer = containerBuilder.Build();

            return new AutofacServiceProvider(ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IDatabaseManager databaseManager, IApplicationLifetime applicationLifetime)
        {
            applicationLifetime.ApplicationStopping.Register(OnShutdown, app);

            if (env.IsDevelopment() || env.IsStaging() || env.IsEnvironment("Debug"))
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }

            app.UseCors(x => x.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseExceptionHandler(options =>
            {
                options.Run(async c => await ExceptionHandler.Handle(c, loggerFactory));
            });

            app.UseCorrelationId();

            app.UseAuthentication();

            #region Swagger

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/v1/swagger.json", "My API");
            });

            #endregion

            app.UseSignalR(routes =>
            {
                routes.MapHub<ChatHub>("/chat");
            });

            app.UseMvc();

            databaseManager.CreateEmptyDatabaseIfNotExistsAsync();
            databaseManager.MigrateToLatestVersion();
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
