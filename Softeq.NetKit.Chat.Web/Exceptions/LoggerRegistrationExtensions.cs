// Developed by Softeq Development Corporation
// http://www.softeq.com

using Autofac;
using CorrelationId;
using Serilog;

namespace Softeq.NetKit.Chat.Web.Exceptions
{
    public static class LoggerRegistrationExtensions
    {
        public static void AddLogger(this ContainerBuilder builder)
        {
            builder.RegisterType<CorrelationContextAccessor>()
                .As<ICorrelationContextAccessor>()
                .SingleInstance();

            builder.RegisterType<CorrelationContextFactory>()
                .As<ICorrelationContextFactory>()
                .InstancePerDependency();

            builder.Register((c, p) =>
                {
                    var correlationContextAccessor = c.Resolve<ICorrelationContextAccessor>();
                    return Log.Logger.ForContext(new CorrelationIdEnricher(correlationContextAccessor));
                })
                .As<ILogger>()
                .InstancePerLifetimeScope();
        }
    }
}