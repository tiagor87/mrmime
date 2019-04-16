using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace MrMime.Core.Aggregates.RequestFakeAgg.Processers
{
    public interface IProcessor
    {
        bool ShouldExecute(KeyValuePair<string, JToken> property);

        void Execute(JObject result, KeyValuePair<string, JToken> property,
            JObject request = null);
    }
}