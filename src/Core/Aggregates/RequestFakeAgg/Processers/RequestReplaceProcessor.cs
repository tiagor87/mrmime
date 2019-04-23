using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace MrMime.Core.Aggregates.RequestFakeAgg.Processers
{
    public class RequestReplaceProcessor : IProcessor
    {
        public bool ShouldExecute(KeyValuePair<string, JToken> property)
        {
            return property.Value != null
                   && Regex.IsMatch(property.Value.ToString(), @"\[[a-zA-Z][\w\._]+\]", RegexOptions.IgnoreCase);
        }

        public void Execute(JObject result, KeyValuePair<string, JToken> property,
            JObject request, IDictionary<string, string> urlParameters)
        {
            var propertyPath = Regex.Match(property.Value.ToString(), @"\[(?<propertyPath>[a-zA-Z][\w\._]+)\]",
                    RegexOptions.IgnoreCase)
                .Groups["propertyPath"].Value.Split('.').ToList();

            var leaf = propertyPath.Last();
            propertyPath.Remove(leaf);

            var obj = request;
            foreach (var path in propertyPath)
            {
                if (obj == null) throw new KeyNotFoundException(path);

                if (obj[path] is JObject jsonObject) obj = jsonObject;
            }

            if (obj == null || obj.ContainsKey(leaf) == false)
            {
                var parameter = urlParameters.First(keyValuePair =>
                    keyValuePair.Key.Equals(leaf, StringComparison.InvariantCultureIgnoreCase));
                obj = new JObject {{leaf, parameter.Value}};
            }

            result[property.Key] = Regex.Replace(property.Value.ToString(), @"\[[a-zA-Z][\w\._]+\]",
                obj[leaf].ToString(),
                RegexOptions.IgnoreCase);
        }
    }
}