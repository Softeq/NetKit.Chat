// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Member;
using Softeq.NetKit.Chat.SignalR.TransportModels.Request.Channel;

namespace Softeq.NetKit.Chat.Tests.Integration.ChatHub
{
    public class ChatHubTestBase
    {
        private readonly string _authenticationServerUrl;

        public ChatHubTestBase(IConfiguration configuration)
        {
            _authenticationServerUrl = configuration["Authentications:Bearer:Authority"];
        }

        protected async Task<string> GetJwtTokenAsync(string userName, string password)
        {
            var httpClient = new HttpClient();

            var values = new Dictionary<string, string>
            {
                {"grant_type", "password"},
                {"username", userName},
                {"password", password},
                {"scope", "api"},
                {"client_id", "ro.client"},
                {"client_secret", "secret"}
            };

            var content = new FormUrlEncodedContent(values);
            var response = await httpClient.PostAsync($"{_authenticationServerUrl}/connect/token", content);
            var responseString = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(responseString);
            return json.Value<string>("access_token");
        }
    }
}