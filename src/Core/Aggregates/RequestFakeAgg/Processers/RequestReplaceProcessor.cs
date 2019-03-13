using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace MrMime.Core.Aggregates.RequestFakeAgg.Processers
{
    public class RequestReplaceProcessor : IProcessor
    {
        public bool ShouldExecute(KeyValuePair<string, object> property)
        {
            return property.Value != null
                   && Regex.IsMatch(property.Value.ToString(), @"\[[a-zA-Z][\w\._]+\]", RegexOptions.IgnoreCase);
        }

        public void Execute(IDictionary<string, object> result, KeyValuePair<string, object> property,
            IDictionary<string, object> request)
        {
            var propertyPath = Regex.Match(property.Value.ToString(), @"\[(?<propertyPath>[a-zA-Z][\w\._]+)\]",
                    RegexOptions.IgnoreCase)
                .Groups["propertyPath"].Value.Split('.').ToList();

            string leaf = propertyPath.Last();
            propertyPath.Remove(leaf);

            var obj = request;
            foreach (var path in propertyPath)
            {
                if (obj[path] is JObject jsonObject)
                {
                    obj = jsonObject.ToObject<IDictionary<string, object>>();
                }
            }

            result[property.Key] = Regex.Replace(property.Value.ToString(), @"\[[a-zA-Z][\w\._]+\]",
                obj[leaf].ToString(),
                RegexOptions.IgnoreCase);
        }
    }
}