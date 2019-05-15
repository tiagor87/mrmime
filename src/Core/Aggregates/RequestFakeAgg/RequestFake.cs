using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using MrMime.Core.Aggregates.RequestFakeAgg.Builders;
using Newtonsoft.Json.Linq;

namespace MrMime.Core.Aggregates.RequestFakeAgg
{
    public class RequestFake
    {
        public string Path { get; set; }
        public string Method { get; set; }
        public string ResponseBuilderType { get; set; }
        public JObject Response { get; set; }
        public int? StatusCode { get; set; }
        public bool IsStreamResponse => ResponseBuilderType == Builders.ResponseBuilderType.Stream;

        public IDictionary<string, string> GetUrlParams(string requestPath)
        {
            if (string.IsNullOrWhiteSpace(requestPath)) return new Dictionary<string, string>();

            var regex = new Regex(Path, RegexOptions.IgnoreCase);
            var groups = regex.Match(requestPath).Groups;
            return regex.GetGroupNames().Where(name => name != "0")
                .ToDictionary(groupName => groupName, groupName => groups[groupName].Value);
        }

        public Stream GetStreamResponse()
        {
            if (!IsStreamResponse) throw new NotSupportedException("Method is allowed just for stream responses.");

            return File.OpenRead(Response["path"].Value<string>());
        }

        public JObject GetResponse(JObject requestValue, string requestUrl = null)
        {
            var builderTypes = new List<string>
            {
                Builders.ResponseBuilderType.RequestReflect,
                Builders.ResponseBuilderType.RequestMerge,
                Builders.ResponseBuilderType.ResponseCopy,
                Builders.ResponseBuilderType.Stream
            };

            switch (builderTypes.IndexOf(ResponseBuilderType))
            {
                case 0:
                    return new ResponseRequestReflectBuilder().FromRequest(requestValue)
                        .WithUrlParams(GetUrlParams(requestUrl)).Build();
                case 1:
                    return new ResponseRequestMergeBuilder().FromRequest(requestValue).MergeWith(Response)
                        .WithUrlParams(GetUrlParams(requestUrl)).Build();
                case 2:
                    return new ResponseCopyBuilder().FromRequest(requestValue).WithResponse(Response)
                        .WithUrlParams(GetUrlParams(requestUrl)).Build();
                case 3:

                default:
                    throw new ArgumentException(
                        $@"Should be ""{Builders.ResponseBuilderType.RequestReflect}"" or ""{Builders.ResponseBuilderType.RequestMerge}"".",
                        nameof(ResponseBuilderType));
            }
        }

        public int GetResponseStatusCode([CallerMemberName] string method = null)
        {
            if (StatusCode.HasValue) return StatusCode.Value;

            if (string.IsNullOrWhiteSpace(method)) return (int) HttpStatusCode.OK;
            switch (method.ToUpper())
            {
                case "POST": return (int) HttpStatusCode.Created;
                case "GET":
                case "PATCH":
                case "PUT": return (int) HttpStatusCode.OK;
                case "DELETE": return (int) HttpStatusCode.NoContent;
                default: return (int) HttpStatusCode.OK;
            }
        }
    }
}