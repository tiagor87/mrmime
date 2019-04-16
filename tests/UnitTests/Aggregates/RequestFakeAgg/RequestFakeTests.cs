using System;
using System.Linq;
using System.Net.Http;
using FluentAssertions;
using MrMime.Core.Aggregates.RequestFakeAgg;
using MrMime.Core.Aggregates.RequestFakeAgg.Builders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace MrMime.UnitTests.Aggregates.RequestFakeAgg
{
    public class RequestFakeTests
    {
        [Fact]
        public void Should_get_response_when_has_null_value()
        {
            var requestJson = @"{
                ""name"": ""Tiago Resende"",
                ""age"": 31,
                ""null_value"": null
            }";

            var requestBody = JsonConvert.DeserializeObject<JObject>(requestJson);
            var request = new RequestFake
            {
                Path = "users",
                Method = HttpMethod.Post.Method,
                Response = null,
                ResponseBuilderType = ResponseBuilderType.RequestReflect
            };

            request.Invoking(x => x.GetResponse(requestBody))
                .Should()
                .NotThrow();
        }
        
        [Fact]
        public void Should_get_response_when_has_array_value()
        {
            var requestJson = @"{
                ""name"": ""Tiago Resende"",
                ""age"": 31,
                ""contacts"": [
                    { ""contact"": ""contact 1"" }
                ]
            }";

            var requestBody = JsonConvert.DeserializeObject<JObject>(requestJson);
            var request = new RequestFake
            {
                Path = "users",
                Method = HttpMethod.Post.Method,
                Response = null,
                ResponseBuilderType = ResponseBuilderType.RequestReflect
            };

            var result = request.GetResponse(requestBody);
            result["contacts"].Should().HaveCount(1);
            result["contacts"].ToList().First()["contact"].Value<string>().Should().Be("contact 1");
        }

        [Fact]
        public void Should_get_response_when_merge_request()
        {
            var requestJson = @"{
            ""name"": ""Tiago Resende"",
            ""age"": 31
            }";
            var responseMockJson = @"{
            ""id"": 1
            }";

            var requestBody = JsonConvert.DeserializeObject<JObject>(requestJson);
            var responseBody = JsonConvert.DeserializeObject<JObject>(responseMockJson);
            var request = new RequestFake
            {
                Path = "users",
                Method = HttpMethod.Post.Method,
                Response = responseBody,
                ResponseBuilderType = ResponseBuilderType.RequestMerge
            };

            var response = request.GetResponse(requestBody);


            response["name"].Value<string>().Should().Be("Tiago Resende");
            response["age"].Value<int>().Should().Be(31);
            response["id"].Value<int>().Should().Be(1);
        }

        [Fact]
        public void Should_get_response_when_reflect_request()
        {
            var requestJson = @"{
            ""name"": ""Tiago Resende"",
            ""age"": 31
            }";

            var requestBody = JsonConvert.DeserializeObject<JObject>(requestJson);
            var request = new RequestFake
            {
                Path = "users",
                Method = HttpMethod.Post.Method,
                Response = null,
                ResponseBuilderType = ResponseBuilderType.RequestReflect
            };

            var response = request.GetResponse(requestBody);
            response.Should().NotBeNull();
            response["name"].Value<string>().Should().Be("Tiago Resende");
            response["age"].Value<int>().Should().Be(31);
        }


        [Fact]
        public void Should_throw_when_builder_type_invalid()
        {
            var request = new RequestFake
            {
                Path = "users",
                Method = "Get",
                ResponseBuilderType = "invalid"
            };

            request
                .Invoking(x => x.GetResponse(new JObject()))
                .Should()
                .Throw<ArgumentException>()
                .And
                .ParamName
                .Should()
                .Be("ResponseBuilderType");
        }
    }
}