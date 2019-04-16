using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FluentAssertions;
using MrMime.Core.Aggregates.RequestFakeAgg.Builders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace MrMime.UnitTests.Aggregates.RequestFakeAgg.Builders
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

            var request = JsonConvert.DeserializeObject<JObject>(requestJson);

            new ResponseRequestReflectBuilder()
                .FromRequest(request)
                .Invoking(x => x.Build())
                .Should()
                .NotThrow();
        }

        [Fact]
        public void Should_parse__partial_guid_tokens()
        {
            var requestJson = @"{
                ""id"": ""cus_{Guid}"",
                ""name"": ""Tiago Resende"",
                ""age"": 31
            }";

            var request = JsonConvert.DeserializeObject<JObject>(requestJson);

            var value = new ResponseRequestReflectBuilder()
                .FromRequest(request)
                .Build();

            Regex.IsMatch(
                value["id"].ToString(),
                @"cus_\w{8}\-(\w{4}\-){3}\w{12}").Should().BeTrue();
            value["name"].Value<string>().Should().Be("Tiago Resende");
            value["age"].Value<int>().Should().Be(31);
        }

        [Fact]
        public void Should_parse_guid_tokens()
        {
            var requestJson = @"{
                ""id"": ""{Guid}"",
                ""name"": ""Tiago Resende"",
                ""age"": 31
            }";

            var request = JsonConvert.DeserializeObject<JObject>(requestJson);

            var value = new ResponseRequestReflectBuilder()
                .FromRequest(request)
                .Build();

            Guid.TryParse(value["id"].ToString(), out _).Should().BeTrue();
            value["name"].Value<string>().Should().Be("Tiago Resende");
            value["age"].Value<int>().Should().Be(31);
        }

        [Fact]
        public void Should_reflect_request()
        {
            var requestJson = @"{
                ""name"": ""Tiago Resende"",
                ""age"": 31
            }";

            var request = JsonConvert.DeserializeObject<JObject>(requestJson);

            var value = new ResponseRequestReflectBuilder()
                .FromRequest(request)
                .Build();

            value["name"].Value<string>().Should().Be("Tiago Resende");
            value["age"].Value<int>().Should().Be(31);
        }
    }
}