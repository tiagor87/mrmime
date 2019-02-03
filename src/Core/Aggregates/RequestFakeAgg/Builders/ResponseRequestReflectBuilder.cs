using System.Collections.Generic;

namespace MrMime.Core.Aggregates.RequestFakeAgg.Builders
{
    public class ResponseRequestReflectBuilder : ResponseBuilder<ResponseRequestReflectBuilder>
    {
        public override IDictionary<string, object> Build()
        {
            return ProcessResponse(RequestBody);
        }
    }
}