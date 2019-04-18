using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
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

        public JObject GetResponse(JObject requestValue)
        {
            var builderTypes = new List<string>
            {
                Builders.ResponseBuilderType.RequestReflect,
                Builders.ResponseBuilderType.RequestMerge,
                Builders.ResponseBuilderType.ResponseCopy
            };

            switch (builderTypes.IndexOf(ResponseBuilderType))
            {
                case 0: return new ResponseRequestReflectBuilder().FromRequest(requestValue).Build();
                case 1: return new ResponseRequestMergeBuilder().FromRequest(requestValue).MergeWith(Response).Build();
                case 2: return new ResponseCopyBuilder().FromRequest(requestValue).WithResponse(Response).Build();
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