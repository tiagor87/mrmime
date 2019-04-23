using System;
using System.Collections.Generic;
using FluentAssertions;
using MrMime.Core.Aggregates.RequestFakeAgg.Processers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace MrMime.UnitTests.Aggregates.RequestFakeAgg.Processors
{
    public class RequestReplaceProcessorTests
    {
        [Theory]
        [InlineData("Property", "[name]", true)]
        [InlineData("Property", "[customer.name]", true)]
        [InlineData("Property", "[customer.customer_name]", true)]
        [InlineData("Property", "[customer.customer_name", false)]
        [InlineData("Property", "customer.customer_name]", false)]
        [InlineData("Property", "[customer.customer_name?]", false)]
        [InlineData("Property", "[.customer_name]", false)]
        [InlineData("Property", "[_customer.customer_name]", false)]
        [InlineData("Property", "[]", false)]
        public void Should_check_when_should_execute(string propertyName, string propertyValue, bool shouldExecute)
        {
            var property = new KeyValuePair<string, JToken>(propertyName, propertyValue);

            var processor = new RequestReplaceProcessor();

            processor.ShouldExecute(property).Should().Be(shouldExecute);
        }

        [Theory]
        [InlineData(@"{""customer"": {""name"": ""Tiago Resende""}}", "CustomerName", "[customer.name]",
            "Tiago Resende")]
        [InlineData(@"{""customer"": {""name"": { ""first_name"": ""Tiago""}}}", "FirstName",
            "[customer.name.first_name]", "Tiago")]
        [InlineData(@"{""customer_name"": ""Tiago""}", "Name", "[customer_name]", "Tiago")]
        public void Should_read_request_value(string json, string propertyName, string replaceProperty,
            string expectedValue)
        {
            var request = JsonConvert.DeserializeObject<JObject>(json);
            var urlParameters = new Dictionary<string, string>();

            var result = new JObject();
            var processor = new RequestReplaceProcessor();
            processor.Execute(result, new KeyValuePair<string, JToken>(propertyName, replaceProperty), request,
                urlParameters);

            result[propertyName].Value<string>().Should().Be(expectedValue);
        }

        [Theory]
        [InlineData(@"{""customer"": {""name2"": ""Tiago Resende""}}", "CustomerName", "[customer.name]")]
        [InlineData(@"{""customername"": ""Tiago Resende""}", "CustomerName", "[customer_name]")]
        public void Should_throw_when_property_not_found(string json, string propertyName, string replaceProperty)
        {
            var request = JsonConvert.DeserializeObject<JObject>(json);
            var urlParameters = new Dictionary<string, string>();

            var result = new JObject();
            var processor = new RequestReplaceProcessor();

            processor
                .Invoking(x =>
                    x.Execute(result, new KeyValuePair<string, JToken>(propertyName, replaceProperty), request,
                        urlParameters))
                .Should()
                .Throw<InvalidOperationException>()
                .WithMessage("Sequence contains no matching element");
        }

        [Theory]
        [InlineData(@"{""customer"": {""name2"": ""Tiago Resende""}}", "CustomerName", "[name]", "name", "Tiago")]
        [InlineData(@"{""customername"": ""Tiago Resende""}", "CustomerName", "[customer_name]", "Customer_Name",
            "Tiago")]
        public void Should_read_url_parameter_when_property_not_found(string json, string propertyName,
            string replaceProperty,
            string urlParameterName, string urlParameterValue)
        {
            var request = JsonConvert.DeserializeObject<JObject>(json);
            var urlParameters = new Dictionary<string, string>
            {
                {urlParameterName, urlParameterValue}
            };

            var result = new JObject();
            var processor = new RequestReplaceProcessor();

            processor.Execute(result, new KeyValuePair<string, JToken>(propertyName, replaceProperty), request,
                urlParameters);

            result[propertyName].Value<string>().Should().Be(urlParameterValue);
        }
    }
}