using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MrMime.Core.Aggregates.RequestFakeAgg.Processers
{
    public class DateTimeProcessor : IProcessor
    {
        public bool ShouldExecute(KeyValuePair<string, object> property)
        {
            return property.Value != null && property.Value.ToString().ToLower().Contains("{datetime}");
        }

        public void Execute(IDictionary<string, object> result, KeyValuePair<string, object> property,
            IDictionary<string, object> _)
        {
            result[property.Key] = Regex.Replace(property.Value.ToString(), @"{datetime}",
                DateTime.UtcNow.ToUniversalTime().ToString("O"),
                RegexOptions.IgnoreCase);
        }
    }
}