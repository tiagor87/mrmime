using FluentAssertions;
using MrMime.Core.Aggregates.RequestFakeAgg.Builders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

            var request = JsonConvert.DeserializeObject<JObject>(requestJson);
            var responseMock = JsonConvert.DeserializeObject<JObject>(responseMockJson);

            var value = new ResponseRequestMergeBuilder()
                .FromRequest(request)
                .MergeWith(responseMock)
                .Build();

            value["name"].Value<string>().Should().Be("Tiago Resende");
            value["age"].Value<int>().Should().Be(31);
            value["id"].Value<int>().Should().Be(1);
            value["address"].Should().BeOfType<JObject>().And.NotBeNull();
            value["address"]["id"].Value<string>().Should().NotBeNull();
            value["address"]["street"].Value<string>().Should().NotBeNull();
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

            var request = JsonConvert.DeserializeObject<JObject>(requestJson);
            var responseMock = JsonConvert.DeserializeObject<JObject>(responseMockJson);

            var value = new ResponseRequestMergeBuilder()
                .FromRequest(request)
                .MergeWith(responseMock)
                .Build();

            value["name"].Value<string>().Should().Be("Tiago Resende");
            value["age"].Value<int>().Should().Be(31);
            value["id"].Value<int>().Should().Be(1);
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

            var request = JsonConvert.DeserializeObject<JObject>(requestJson);
            var responseMock = JsonConvert.DeserializeObject<JObject>(responseMockJson);

            new ResponseRequestMergeBuilder()
                .FromRequest(request)
                .MergeWith(responseMock)
                .Invoking(x => x.Build())
                .Should()
                .NotThrow();
        }
    }
}