using System;
using System.Collections.Generic;
using System.Net.Http;
using FluentAssertions;
using MrMime.Core.Aggregates.RequestFakeAgg;
using MrMime.Core.Aggregates.RequestFakeAgg.Builders;
using Newtonsoft.Json;
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

            var requestBody = JsonConvert.DeserializeObject<IDictionary<string, object>>(requestJson);
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
        public void Should_get_response_when_merge_request()
        {
            var requestJson = @"{
            ""name"": ""Tiago Resende"",
            ""age"": 31
            }";
            var responseMockJson = @"{
            ""id"": 1
            }";

            var requestBody = JsonConvert.DeserializeObject<IDictionary<string, object>>(requestJson);
            var responseBody = JsonConvert.DeserializeObject<IDictionary<string, object>>(responseMockJson);
            var request = new RequestFake
            {
                Path = "users",
                Method = HttpMethod.Post.Method,
                Response = responseBody,
                ResponseBuilderType = ResponseBuilderType.RequestMerge
            };

            var response = request.GetResponse(requestBody);


            response["name"].Should().Be("Tiago Resende");
            response["age"].Should().Be(31);
            response["id"].Should().Be(1);
        }

        [Fact]
        public void Should_get_response_when_reflect_request()
        {
            var requestJson = @"{
            ""name"": ""Tiago Resende"",
            ""age"": 31
            }";

            var requestBody = JsonConvert.DeserializeObject<IDictionary<string, object>>(requestJson);
            var request = new RequestFake
            {
                Path = "users",
                Method = HttpMethod.Post.Method,
                Response = null,
                ResponseBuilderType = ResponseBuilderType.RequestReflect
            };

            var response = request.GetResponse(requestBody);
            response.Should().NotBeNull();
            response.Should().ContainKeys("name", "age");
            response.Values.Should().ContainInOrder("Tiago Resende", 31);
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
                .Invoking(x => x.GetResponse(new Dictionary<string, object>()))
                .Should()
                .Throw<ArgumentException>()
                .And
                .ParamName
                .Should()
                .Be("ResponseBuilderType");
        }
    }
}