using System;
using System.Collections.Generic;
using System.Globalization;
using FluentAssertions;
using MrMime.Core.Aggregates.RequestFakeAgg.Builders;
using Newtonsoft.Json;
using Xunit;

namespace MrMime.UnitTests.Aggregates.RequestFakeAgg.Builders
{
    public class ResponseCopyBuilderTests
    {
        [Fact]
        public void Should_process_response()
        {
            var requestJson = @"{
                ""AmountInCents"": 1490,
                ""AutomaticCapture"": false,
                ""Card"": {
                    ""CardBrand"": ""Visa"",
                    ""CardNumber"": ""4000000000000077"",
                    ""ExpMonth"": 1,
                    ""ExpYear"": 2020,
                    ""HolderDocumentNumber"": null,
                    ""HolderName"": ""Tony Stark"",
                    ""SecurityCode"": ""651""
                },
                ""Installment"": 0,
                ""SoftDescriptor"": null,
                ""TransactionKeyToAcquirer"": ""aa063cd5-cddb-4a2c-9bba-98bdb9d1957e"",
                ""TransactionReference"": ""tran_O2g5Z18H3YUmJPEN"",
                ""NotifieUrl"": ""http://pruu.herokuapp.com/dump/testeget""
            }";
            var responseMockJson = @"{
                ""AmountInCents"": ""[AmountInCents]"",
                ""AuthorizedAmountInCents"": ""[AmountInCents]"",
                ""TransactionStatus"": ""Authorized"",
                ""TransactionIdentifier"": ""{Guid}"",
                ""UniqueSequentialNumber"": ""{Guid}"",
                ""SoftDescriptor"": null,
                ""CreateDate"": ""{DateTime}"",
                ""AuthorizedDate"": ""{DateTime}"",
                ""AuthorizationCode"": ""291"",
                ""TransactionKey"": ""{Guid}"",
                ""TransactionKeyToAcquirer"": ""[TransactionKeyToAcquirer]"",
                ""TransactionReference"": ""[TransactionReference]"",
                ""AcquirerMessage"": ""Transação authorizada com sucesso"",
                ""AcquirerReturnCode"": ""00""
            }";

            var request = JsonConvert.DeserializeObject<IDictionary<string, object>>(requestJson);
            var responseMock = JsonConvert.DeserializeObject<IDictionary<string, object>>(responseMockJson);

            var value = new ResponseCopyBuilder()
                .FromRequest(request)
                .WithResponse(responseMock)
                .Build();

            value.Keys.Should().HaveCount(14);

            value["AmountInCents"].Should().Be("1490");
            value["AuthorizedAmountInCents"].Should().Be("1490");
            value["TransactionStatus"].Should().Be("Authorized");
            Guid.TryParse(value["TransactionIdentifier"].ToString(), out _).Should().BeTrue();
            Guid.TryParse(value["UniqueSequentialNumber"].ToString(), out _).Should().BeTrue();
            value["SoftDescriptor"].Should().BeNull();
            DateTime.TryParse(value["CreateDate"].ToString(), null, DateTimeStyles.AdjustToUniversal,
                out var createDate).Should().BeTrue();
            createDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            DateTime.TryParse(value["AuthorizedDate"].ToString(), null, DateTimeStyles.AdjustToUniversal,
                out var authorizedDate).Should().BeTrue();
            authorizedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            value["AuthorizationCode"].Should().Be("291");
            Guid.TryParse(value["TransactionKey"].ToString(), out _).Should().BeTrue();
            value["TransactionKeyToAcquirer"].Should().Be("aa063cd5-cddb-4a2c-9bba-98bdb9d1957e");
            value["TransactionReference"].Should().Be("tran_O2g5Z18H3YUmJPEN");
            value["AcquirerMessage"].Should().Be("Transação authorizada com sucesso");
            value["AcquirerReturnCode"].Should().Be("00");
        }
    }
}