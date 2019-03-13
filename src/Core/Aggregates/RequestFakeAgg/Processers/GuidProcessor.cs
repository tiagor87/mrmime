using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MrMime.Core.Aggregates.RequestFakeAgg.Processers
{
    public class GuidProcessor : IProcessor
    {
        public bool ShouldExecute(KeyValuePair<string, object> property)
        {
            return property.Value != null && property.Value.ToString().ToLower().Contains("{guid}");
        }

        public void Execute(IDictionary<string, object> result, KeyValuePair<string, object> property,
            IDictionary<string, object> _)
        {
            result[property.Key] = Regex.Replace(property.Value.ToString(), @"{guid}", Guid.NewGuid().ToString(),
                RegexOptions.IgnoreCase);
        }
    }
}