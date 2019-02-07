using System;
using System.Collections.Generic;
using FluentAssertions;
using MrMime.Core.Aggregates.RequestFakeAgg.Builders;
using Newtonsoft.Json;
using Xunit;

namespace MrMime.Core.Tests.Aggregates.RequestFakeAgg.Builders
{
    public class ResponseRequestReflectBuilderTests
    {
        [Fact]
        public void Should_not_throw_when_null_value()
        {
            var requestJson = @"{
                ""name"": ""Tiago Resende"",
                ""age"": 31,
                ""address"": null
            }";

            var request = JsonConvert.DeserializeObject<IDictionary<string, object>>(requestJson);

            new ResponseRequestReflectBuilder()
                .FromRequest(request)
                .Invoking(x => x.Build())
                .Should()
                .NotThrow();
        }

        [Fact]
        public void Should_parse_guid_tokens()
        {
            var requestJson = @"{
                ""id"": ""{Guid}"",
                ""name"": ""Tiago Resende"",
                ""age"": 31
            }";

            var request = JsonConvert.DeserializeObject<IDictionary<string, object>>(requestJson);

            var value = new ResponseRequestReflectBuilder()
                .FromRequest(request)
                .Build();

            value["id"].Should()
                .BeOfType<Guid>()
                .And
                .NotBe(Guid.Empty);
            value["name"].Should().Be("Tiago Resende");
            value["age"].Should().Be(31);
        }

        [Fact]
        public void Should_reflect_request()
        {
            var requestJson = @"{
                ""name"": ""Tiago Resende"",
                ""age"": 31
            }";

            var request = JsonConvert.DeserializeObject<IDictionary<string, object>>(requestJson);

            var value = new ResponseRequestReflectBuilder()
                .FromRequest(request)
                .Build();

            value["name"].Should().Be("Tiago Resende");
            value["age"].Should().Be(31);
        }
    }
}