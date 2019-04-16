using Newtonsoft.Json.Linq;

namespace MrMime.Core.Aggregates.RequestFakeAgg.Builders
{
    public class ResponseRequestReflectBuilder : ResponseBuilder<ResponseRequestReflectBuilder>
    {
        public override JObject Build()
        {
            return ProcessResponse(RequestBody);
        }
    }
}