using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace MrMime.Core.Aggregates.RequestFakeAgg.Builders
{
    public class ResponseCopyBuilder : ResponseBuilder<ResponseCopyBuilder>
    {
        private JObject _response;

        public ResponseCopyBuilder WithResponse(JObject response)
        {
            _response = response;
            return this;
        }

        public override JObject Build()
        {
            return ProcessResponse(_response);
        }
    }
}