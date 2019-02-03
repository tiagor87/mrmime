using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace MrMime.Core.Aggregates.RequestFakeAgg.Builders
{
    public abstract class ResponseBuilder<TBuilder> where TBuilder : ResponseBuilder<TBuilder>
    {
        public IDictionary<string, object> RequestBody { get; private set; }

        public TBuilder FromRequest(IDictionary<string, object> requestBody)
        {
            RequestBody = requestBody ?? throw new ArgumentNullException(nameof(requestBody));
            return (TBuilder) this;
        }

        public abstract IDictionary<string, object> Build();

        protected IDictionary<string, object> ProcessResponse(IDictionary<string, object> dictionary)
        {
            var result = new Dictionary<string, object>();
            foreach (var pair in dictionary)
                if (pair.Value.ToString().Equals("{Guid}", StringComparison.InvariantCultureIgnoreCase))
                {
                    result[pair.Key] = Guid.NewGuid();
                }
                else if (pair.Value is JObject obj)
                {
                    var dic = obj.ToObject<IDictionary<string, object>>();
                    result.Add(pair.Key, ProcessResponse(dic));
                }
                else
                {
                    result[pair.Key] = pair.Value;
                }

            return result;
        }
    }
}