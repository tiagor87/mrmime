using System.Collections.Generic;

namespace MrMime.Core.Aggregates.RequestFakeAgg.Processers
{
    public interface IProcessor
    {
        bool ShouldExecute(KeyValuePair<string, object> property);

        void Execute(IDictionary<string, object> result, KeyValuePair<string, object> property,
            IDictionary<string, object> request);
    }
}