using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Softeq.NetKit.Chat.Web;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Integration.Rest
{
    public class IntegrationTests : ICollectionFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public IntegrationTests()
        {
            var factory = new WebApplicationFactory<Startup>();
            _client = factory.CreateClient();
        }

        public HttpClient Client => _client;
    }
}
