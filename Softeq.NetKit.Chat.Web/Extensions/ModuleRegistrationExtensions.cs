// Developed by Softeq Development Corporation
// http://www.softeq.com

using Autofac;
using Softeq.NetKit.Chat.Data.Cloud.Azure;
using Softeq.NetKit.Chat.Data.Persistent.Sql;
using Softeq.NetKit.Chat.Domain.Services;
using Softeq.NetKit.Chat.SignalR;

namespace Softeq.NetKit.Chat.Web.Extensions
{
    public static class ModuleRegistrationExtensions
    {
        public static void RegisterSolutionModules(this ContainerBuilder builder)
        {
            builder.RegisterModule<SignalRDiModule>();
            builder.RegisterModule<DomainServicesDiModule>();
            builder.RegisterModule<DataPersistentSqlDiModule>();
            builder.RegisterModule<DataCloudAzureDiModule>();
        }
    }
}