// Developed by Softeq Development Corporation
// http://www.softeq.com

using Autofac;

namespace Softeq.NetKit.Chat.Application.Services
{
    public class ApplicationServicesDiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            RegisterApplicationServices(builder);
        }

        private static void RegisterApplicationServices(ContainerBuilder builder)
        {
            builder.RegisterType<ChatSystemMessagesService>().As<IChatSystemMessagesService>();
        }
    }
}
