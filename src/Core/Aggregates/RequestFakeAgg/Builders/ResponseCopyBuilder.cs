using System.Collections.Generic;

namespace MrMime.Core.Aggregates.RequestFakeAgg.Builders
{
    public class ResponseCopyBuilder : ResponseBuilder<ResponseCopyBuilder>
    {
        private IDictionary<string, object> _response;

        public ResponseCopyBuilder WithResponse(IDictionary<string, object> response)
        {
            _response = response;
            return this;
        }

        public override IDictionary<string, object> Build()
        {
            return ProcessResponse(_response);
        }
    }
}