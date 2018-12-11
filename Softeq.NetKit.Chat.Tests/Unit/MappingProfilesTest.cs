// Developed by Softeq Development Corporation
// http://www.softeq.com

using AutoMapper;
using Softeq.NetKit.Chat.Domain.Services.Mappings;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit
{
    public class MappingProfilesTest
    {
        [Fact]
        public void ShouldHaveValidMappingProfiles()
        {
            Mapper.Initialize(cfg => { cfg.AddProfile(typeof(DomainServicesMappingProfile)); });

            Mapper.Configuration.AssertConfigurationIsValid();
        }
    }
}