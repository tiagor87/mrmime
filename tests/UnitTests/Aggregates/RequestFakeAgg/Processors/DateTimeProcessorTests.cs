using System.Collections.Generic;
using System.Text.RegularExpressions;
using FluentAssertions;
using MrMime.Core.Aggregates.RequestFakeAgg.Processers;
using Newtonsoft.Json.Linq;
using Xunit;

namespace MrMime.UnitTests.Aggregates.RequestFakeAgg.Processors
{
    public class DateTimeProcessorTests
    {
        [Theory]
        [InlineData("Property", "{datetime}", true)]
        [InlineData("Property", "cus_{datetime}", true)]
        [InlineData("Property", "cus_{datetime}_tomer", true)]
        [InlineData("Property", "{date}", false)]
        [InlineData("Property", "{datetim", false)]
        [InlineData("Property", "{datetimee}", false)]
        [InlineData("Property", "{datetime", false)]
        public void Should_check_when_should_execute(string propertyName, string propertyValue, bool shouldExecute)
        {
            var property = new KeyValuePair<string, JToken>(propertyName, propertyValue);

            var processor = new DateTimeProcessor();

            processor.ShouldExecute(property).Should().Be(shouldExecute);
        }

        [Theory]
        [InlineData("CustomerDate", "cus_{datetime}", @"cus_\d{4}(\-\d{2}){2}T\d{2}(:\d{2}){2}\.\d+Z")]
        [InlineData("CustomerDate", "{datetime}", @"\d{4}(\-\d{2}){2}T\d{2}(:\d{2}){2}\.\d+Z")]
        [InlineData("CustomerDate", "{datetime}_test", @"\d{4}(\-\d{2}){2}T\d{2}(:\d{2}){2}\.\d+Z_test")]
        public void Should_read_request_value(string propertyName, string valueProperty, string pattern)
        {
            var result = new JObject();
            var processor = new DateTimeProcessor();
            processor.Execute(result, new KeyValuePair<string, JToken>(propertyName, valueProperty));

            result.Should().ContainKey(propertyName);
            Regex.IsMatch(result[propertyName].ToString(), pattern, RegexOptions.IgnoreCase)
                .Should()
                .BeTrue();
        }
    }
}