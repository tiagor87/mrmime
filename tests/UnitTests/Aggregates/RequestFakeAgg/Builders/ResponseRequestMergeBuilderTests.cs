using System.Collections.Generic;
using FluentAssertions;
using MrMime.Core.Aggregates.RequestFakeAgg.Builders;
using Newtonsoft.Json;
using Xunit;

namespace MrMime.UnitTests.Aggregates.RequestFakeAgg.Builders
{
    public class ResponseRequestMergeBuilderTests
    {
        [Fact]
        public void Should_merge_a_complex_object()
        {
            var requestJson = @"{
                ""name"": ""Tiago Resende"",
                ""age"": 31,
                ""address"": {
                    ""street"": ""Rua A. C."",
                    ""number"": ""100""
                }
            }";
            var responseMockJson = @"{
                ""id"": 1,
                ""address"": {
                    ""id"": 1
                }
            }";

            var request = JsonConvert.DeserializeObject<IDictionary<string, object>>(requestJson);
            var responseMock = JsonConvert.DeserializeObject<IDictionary<string, object>>(responseMockJson);

            var value = new ResponseRequestMergeBuilder()
                .FromRequest(request)
                .MergeWith(responseMock)
                .Build();

            value["name"].Should().Be("Tiago Resende");
            value["age"].Should().Be(31);
            value["id"].Should().Be(1);
            value["address"].Should().BeAssignableTo<IDictionary<string, object>>();
            value["address"].As<IDictionary<string, object>>()
                .Should()
                .ContainKeys("id", "street");
            value["address"].As<IDictionary<string, object>>()
                .Values
                .Should()
                .Contain("Rua A. C.", "100", 1);
        }

        [Fact]
        public void Should_merge_a_simple_object()
        {
            var requestJson = @"{
            ""name"": ""Tiago Resende"",
            ""age"": 31
            }";
            var responseMockJson = @"{
            ""id"": 1
            }";

            var request = JsonConvert.DeserializeObject<IDictionary<string, object>>(requestJson);
            var responseMock = JsonConvert.DeserializeObject<IDictionary<string, object>>(responseMockJson);

            var value = new ResponseRequestMergeBuilder()
                .FromRequest(request)
                .MergeWith(responseMock)
                .Build();

            value["name"].Should().Be("Tiago Resende");
            value["age"].Should().Be(31);
            value["id"].Should().Be(1);
        }

        [Fact]
        public void Should_not_throw_when_null_value()
        {
            var requestJson = @"{
                ""name"": ""Tiago Resende"",
                ""age"": 31,
                ""address"": null
            }";
            var responseMockJson = @"{
                ""id"": ""{Guid}""
            }";

            var request = JsonConvert.DeserializeObject<IDictionary<string, object>>(requestJson);
            var responseMock = JsonConvert.DeserializeObject<IDictionary<string, object>>(responseMockJson);

            new ResponseRequestMergeBuilder()
                .FromRequest(request)
                .MergeWith(responseMock)
                .Invoking(x => x.Build())
                .Should()
                .NotThrow();
        }
    }
}