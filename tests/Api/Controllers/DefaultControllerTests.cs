using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace MrMime.Api.Tests.Controllers
{
    public class DefaultControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public DefaultControllerTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory.WithWebHostBuilder(options =>
            {
                options.UseContentRoot(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            }) ?? throw new ArgumentNullException(nameof(factory));
        }

        [Theory]
        [InlineData("/customers?id=1&name=Test", "1", "Test")]
        [InlineData("/customers/1/cards?id=2&name=Test2", "2", "Test2")]
        [InlineData("/customers/filter?id=3&name=Test3", "3", "Test3")]
        public async Task Should_reflect_query_params_when_get(string path, string id, string name)
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync(path);
            var content =
                JsonConvert.DeserializeObject<IDictionary<string, object>>(await response.Content.ReadAsStringAsync());

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().ContainKeys(nameof(id), nameof(name));
            content[nameof(id)].Should().Be(id);
            content[nameof(name)].Should().Be(name);
        }

        [Theory]
        [InlineData("/customers")]
        [InlineData("/customers/1/cards")]
        [InlineData("/customers/filter")]
        public async Task Should_return_ok_when_query_params_is_empty(string path)
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync(path);
            var content =
                JsonConvert.DeserializeObject<IDictionary<string, object>>(await response.Content.ReadAsStringAsync());

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().BeEmpty();
        }

        [Theory]
        [InlineData("/customers/reflect")]
        [InlineData("/customers/1/cards/reflect")]
        public async Task Should_return_created_and_body_when_post(string path)
        {
            var client = _factory.CreateClient();
            var json = JsonConvert.SerializeObject(new
            {
                Name = "Tiago",
                Age = 31
            });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(path, content);
            var responseContent =
                JsonConvert.DeserializeObject<IDictionary<string, object>>(await response.Content.ReadAsStringAsync());

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            responseContent.Should().NotBeNull();
            responseContent["Name"].Should().Be("Tiago");
            responseContent["Age"].Should().Be(31);
        }

        [Theory]
        [InlineData("/customers/reflect")]
        [InlineData("/customers/1/cards/reflect")]
        public async Task Should_return_ok_and_body_when_put(string path)
        {
            var client = _factory.CreateClient();
            var json = JsonConvert.SerializeObject(new
            {
                Name = "Tiago",
                Age = 31
            });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync(path, content);
            var responseContent =
                JsonConvert.DeserializeObject<IDictionary<string, object>>(await response.Content.ReadAsStringAsync());

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseContent.Should().NotBeNull();
            responseContent["Name"].Should().Be("Tiago");
            responseContent["Age"].Should().Be(31);
        }

        [Theory]
        [InlineData("/customers/reflect")]
        [InlineData("/customers/1/cards/reflect")]
        public async Task Should_return_no_content_when_delete(string path)
        {
            var client = _factory.CreateClient();

            var response = await client.DeleteAsync(path);

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}