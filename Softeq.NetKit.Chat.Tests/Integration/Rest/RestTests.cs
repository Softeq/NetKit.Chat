using System.Threading.Tasks;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Integration.Rest
{
    public class RestTests : BaseTest
    {
        private readonly IntegrationTests _testHostFixture;

        public RestTests()
        {
            _testHostFixture = new IntegrationTests();
        }

        [Fact]
        public async Task Test1()
        {
            var response = await _testHostFixture.Client.GetStringAsync("index");
        }
    }
}

