// Developed by Softeq Development Corporation
// http://www.softeq.com

using Autofac;
using CommonModule = Softeq.NetKit.Chat.Common.DIModule;
using DataRepositoriesModule = Softeq.NetKit.Chat.Data.Repositories.DIModule;
using DomainServicesModule = Softeq.NetKit.Chat.Domain.Services.DIModule;
using InfrastructureSignalRModule = Softeq.NetKit.Chat.Infrastructure.SignalR.DIModule;
using WebModule = Softeq.NetKit.Chat.Web.App.DI.DIModule;

namespace Softeq.NetKit.Chat.Web.App.DI
{
    internal static class DIModulesManager
    {
        public static void RegisterModules(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterModule<CommonModule>();
            containerBuilder.RegisterModule<DataRepositoriesModule>();
            containerBuilder.RegisterModule<DomainServicesModule>();
            containerBuilder.RegisterModule<InfrastructureSignalRModule>();
            containerBuilder.RegisterModule<WebModule>();
        }
    }
}
