using System.Collections.Generic;
using System.Text.RegularExpressions;
using FluentAssertions;
using MrMime.Core.Aggregates.RequestFakeAgg.Processers;
using Newtonsoft.Json.Linq;
using Xunit;

namespace MrMime.UnitTests.Aggregates.RequestFakeAgg.Processors
{
    public class GuidProcessorTests
    {
        [Theory]
        [InlineData("Property", "{guid}", true)]
        [InlineData("Property", "cus_{guid}", true)]
        [InlineData("Property", "{gui}", false)]
        [InlineData("Property", "{guid", false)]
        [InlineData("Property", "{guidd}", false)]
        public void Should_check_when_should_execute(string propertyName, string propertyValue, bool shouldExecute)
        {
            var property = new KeyValuePair<string, JToken>(propertyName, propertyValue);

            var processor = new GuidProcessor();

            processor.ShouldExecute(property).Should().Be(shouldExecute);
        }

        [Theory]
        [InlineData("CustomerId", "cus_{guid}", @"cus_\w{8}\-(\w{4}\-){3}\w{12}")]
        [InlineData("CustomerId", "{guid}", @"\w{8}\-(\w{4}\-){3}\w{12}")]
        [InlineData("CustomerId", "{guid}_test", @"\w{8}\-(\w{4}\-){3}\w{12}_test")]
        public void Should_read_request_value(string propertyName, string valueProperty, string pattern)
        {
            var result = new JObject();
            var processor = new GuidProcessor();
            processor.Execute(result, new KeyValuePair<string, JToken>(propertyName, valueProperty));

            result.Should().ContainKey(propertyName);
            Regex.IsMatch(result[propertyName].ToString(), pattern, RegexOptions.IgnoreCase)
                .Should()
                .BeTrue();
        }
    }
}