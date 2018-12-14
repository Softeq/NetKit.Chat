// Developed by Softeq Development Corporation
// http://www.softeq.com

using Autofac;
using AutoMapper;
using Softeq.NetKit.Chat.Domain.Services.Mappings;

namespace Softeq.NetKit.Chat.Web
{
    public class WebDiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            RegisterAutomapperConfiguration(builder);
        }

        private void RegisterAutomapperConfiguration(ContainerBuilder builder)
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new DomainServicesMappingProfile());
            });

            builder.Register(c =>
            {
                var context = c.Resolve<IComponentContext>();
                return mapperConfiguration.CreateMapper(context.Resolve);
            }).As<IMapper>();
        }
    }
}