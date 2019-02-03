using System;
using System.Collections.Generic;
using MrMime.Core.Aggregates.RequestFakeAgg.Builders;

namespace MrMime.Core.Aggregates.RequestFakeAgg
{
    public class RequestFake
    {
        public string Path { get; set; }
        public string Method { get; set; }
        public string ResponseBuilderType { get; set; }
        public IDictionary<string, object> Response { get; set; }

        public IDictionary<string, dynamic> GetResponse(IDictionary<string, dynamic> requestValue)
        {
            var builderTypes = new List<string>
            {
                Builders.ResponseBuilderType.RequestReflect,
                Builders.ResponseBuilderType.RequestMerge
            };

            switch (builderTypes.IndexOf(ResponseBuilderType))
            {
                case 0: return new ResponseRequestReflectBuilder().FromRequest(requestValue).Build();
                case 1: return new ResponseRequestMergeBuilder().FromRequest(requestValue).MergeWith(Response).Build();
                default:
                    throw new ArgumentException(
                        $@"Should be ""{Builders.ResponseBuilderType.RequestReflect}"" or ""{Builders.ResponseBuilderType.RequestMerge}"".",
                        nameof(ResponseBuilderType));
            }
        }
    }
}