using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace MrMime.Core.Aggregates.RequestFakeAgg.Builders
{
    public class ResponseRequestMergeBuilder : ResponseBuilder<ResponseRequestMergeBuilder>
    {
        public JObject MergeObject { get; private set; }

        public ResponseRequestMergeBuilder MergeWith(JObject mergeObject)
        {
            MergeObject = mergeObject;
            return this;
        }

        public override JObject Build()
        {
            RequestBody.Merge(MergeObject, new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Union
            });
            return ProcessResponse(RequestBody);
        }
    }
}