using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using MrMime.Api;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace MrMime.IntegrationTests.Controllers
{
    public class DefaultControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        public DefaultControllerTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory.WithWebHostBuilder(options =>
            {
                options.UseContentRoot(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            }) ?? throw new ArgumentNullException(nameof(factory));
        }

        private readonly WebApplicationFactory<Startup> _factory;

        [Theory]
        [InlineData("/customers?id=1&name=Test", "1", "Test")]
        [InlineData("/customers/1/cards?id=2&name=Test2", "2", "Test2")]
        [InlineData("/customers/filter?id=3&name=Test3", "3", "Test3")]
        public async Task Should_reflect_query_params_when_get(string path, string id, string name)
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync(path);
            var content =
                JsonConvert.DeserializeObject<JObject>(await response.Content.ReadAsStringAsync());

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().ContainKeys(nameof(id), nameof(name));
            content[nameof(id)].Value<string>().Should().Be(id);
            content[nameof(name)].Value<string>().Should().Be(name);
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

        [Theory]
        [InlineData("ok", HttpStatusCode.OK)]
        [InlineData("created", HttpStatusCode.Created)]
        [InlineData("badRequest", HttpStatusCode.BadRequest)]
        [InlineData("internalServerError", HttpStatusCode.InternalServerError)]
        public async Task Should_return_chosen_status_code(string url, HttpStatusCode statusCode)
        {
            var client = _factory.CreateClient();

            var response = await client.PostAsJsonAsync(url, new { });

            response.StatusCode.Should().Be(statusCode);
        }

        [Fact]
        public async Task Should_copy_response_execute_processors()
        {
            var client = _factory.CreateClient();
            var json = JsonConvert.SerializeObject(new
            {
                AmountInCents = 1490,
                AutomaticCapture = false,
                Card = new
                {
                    CardBrand = "Visa",
                    CardNumber = "4000000000000077",
                    ExpMonth = 1,
                    ExpYear = 2020,
                    HolderDocumentNumber = (object) null,
                    HolderName = "Tony Stark",
                    SecurityCode = "651"
                },
                Installment = 0,
                SoftDescriptor = (object) null,
                TransactionKeyToAcquirer = "aa063cd5-cddb-4a2c-9bba-98bdb9d1957e",
                TransactionReference = "tran_O2g5Z18H3YUmJPEN",
                NotifieUrl = "http://pruu.herokuapp.com/dump/testeget"
            });
            var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

            var response = await client.PostAsync("/v11/authorize", content);
            var responseContent =
                JsonConvert.DeserializeObject<IDictionary<string, object>>(await response.Content.ReadAsStringAsync());

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            responseContent.Should().NotBeNull();

            responseContent.Keys.Should().HaveCount(14);

            responseContent["AmountInCents"].Should().Be("1490");
            responseContent["AuthorizedAmountInCents"].Should().Be("1490");
            responseContent["TransactionStatus"].Should().Be("Authorized");
            Guid.TryParse(responseContent["TransactionIdentifier"].ToString(), out _).Should().BeTrue();
            Guid.TryParse(responseContent["UniqueSequentialNumber"].ToString(), out _).Should().BeTrue();
            responseContent["SoftDescriptor"].Should().BeNull();
            DateTime.TryParse(responseContent["CreateDate"].ToString(), null, DateTimeStyles.AdjustToUniversal,
                out var createDate).Should().BeTrue();
            createDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            DateTime.TryParse(responseContent["AuthorizedDate"].ToString(), null, DateTimeStyles.AdjustToUniversal,
                out var authorizedDate).Should().BeTrue();
            authorizedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            responseContent["AuthorizationCode"].Should().Be("291");
            Guid.TryParse(responseContent["TransactionKey"].ToString(), out _).Should().BeTrue();
            responseContent["TransactionKeyToAcquirer"].Should().Be("aa063cd5-cddb-4a2c-9bba-98bdb9d1957e");
            responseContent["TransactionReference"].Should().Be("tran_O2g5Z18H3YUmJPEN");
            responseContent["AcquirerMessage"].Should().Be("Transação authorizada com sucesso");
            responseContent["AcquirerReturnCode"].Should().Be("00");
        }
    }
}