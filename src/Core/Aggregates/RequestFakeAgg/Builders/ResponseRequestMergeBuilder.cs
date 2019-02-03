using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace MrMime.Core.Aggregates.RequestFakeAgg.Builders
{
    public class ResponseRequestMergeBuilder : ResponseBuilder<ResponseRequestMergeBuilder>
    {
        public IDictionary<string, object> MergeObject { get; private set; }

        public ResponseRequestMergeBuilder MergeWith(IDictionary<string, object> mergeObject)
        {
            MergeObject = mergeObject;
            return this;
        }

        public override IDictionary<string, object> Build()
        {
            var obj = JObject.FromObject(RequestBody);
            var mergeObj = JObject.FromObject(MergeObject);
            obj.Merge(mergeObj, new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Union
            });
            return ProcessResponse(obj.ToObject<IDictionary<string, object>>());
        }
    }
}