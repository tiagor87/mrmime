using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace MrMime.Core.Aggregates.RequestFakeAgg.Processers
{
    public class DateTimeProcessor : IProcessor
    {
        public bool ShouldExecute(KeyValuePair<string, JToken> property)
        {
            return property.Value != null && property.Value.ToString().ToLower().Contains("{datetime}");
        }

        public void Execute(JObject result, KeyValuePair<string, JToken> property,
            JObject request = null)
        {
            result[property.Key] = Regex.Replace(property.Value.ToString(), @"{datetime}",
                DateTime.UtcNow.ToUniversalTime().ToString("O"),
                RegexOptions.IgnoreCase);
        }
    }
}