using System.Collections.Generic;
using FluentAssertions;
using MrMime.Core.Aggregates.RequestFakeAgg.Processers;
using Newtonsoft.Json;
using Xunit;

namespace MrMime.Core.Tests.Aggregates.RequestFakeAgg.Processors
{
    public class RequestReplaceProcessorTests
    {
        [Theory]
        [InlineData(@"{""customer"": {""name"": ""Tiago Resende""}}", "CustomerName", "[customer.name]",
            "Tiago Resende")]
        [InlineData(@"{""customer"": {""name"": { ""first_name"": ""Tiago""}}}", "FirstName",
            "[customer.name.first_name]", "Tiago")]
        [InlineData(@"{""customer_name"": ""Tiago""}", "Name", "[customer_name]", "Tiago")]
        public void Test(string json, string propertyName, string replaceProperty, string expectedValue)
        {
            var request = JsonConvert.DeserializeObject<IDictionary<string, object>>(json);

            var result = new Dictionary<string, object>();
            var processor = new RequestReplaceProcessor();
            processor.Process(result, new KeyValuePair<string, object>(propertyName, replaceProperty), request);

            result.Should()
                .ContainKey(propertyName)
                .And
                .ContainValue(expectedValue);
        }
    }
}