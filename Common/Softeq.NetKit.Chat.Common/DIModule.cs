// // Developed by Softeq Development Corporation
// // http://www.softeq.com

using Autofac;
using CorrelationId;
using Serilog;
using Softeq.NetKit.Chat.Common.Log;
using SerilogLog = Serilog.Log;

namespace Softeq.NetKit.Chat.Common
{
    public class DIModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<CorrelationContextAccessor>()
                .As<ICorrelationContextAccessor>()
                .SingleInstance();

            builder
                .RegisterType<CorrelationContextFactory>()
                .As<ICorrelationContextFactory>()
                .InstancePerDependency();

            builder
                .Register((c, p) =>
                {
                    var correlationContextAccessor = c.Resolve<ICorrelationContextAccessor>();
                    return SerilogLog.Logger.ForContext(new CorrelationIdEnricher(correlationContextAccessor));
                })
                .As<ILogger>()
                .InstancePerLifetimeScope();
        }
    }
}