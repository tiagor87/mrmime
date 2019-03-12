using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace MrMime.Core.Aggregates.RequestFakeAgg.Processers
{
    public interface IProcessor
    {
        void Process(IDictionary<string, object> result, KeyValuePair<string, object> property,
            IDictionary<string, object> request);
    }

    public class NullProcessor : IProcessor
    {
        public void Process(IDictionary<string, object> result, KeyValuePair<string, object> property,
            IDictionary<string, object> _)
        {
            if (property.Value == null)
            {
                result[property.Key] = property.Value;
            }
        }
    }

    public class GuidProcessor : IProcessor
    {
        public void Process(IDictionary<string, object> result, KeyValuePair<string, object> property,
            IDictionary<string, object> _)
        {
            if (property.Value.ToString().ToLower().Contains("{guid}"))
            {
                result[property.Key] = Regex.Replace(property.Value.ToString(), @"{guid}", Guid.NewGuid().ToString(),
                    RegexOptions.IgnoreCase);
            }
        }
    }

    public class DateTimeProcessor : IProcessor
    {
        public void Process(IDictionary<string, object> result, KeyValuePair<string, object> property,
            IDictionary<string, object> _)
        {
            if (property.Value.ToString().ToLower().Contains("{datetime}"))
            {
                result[property.Key] = Regex.Replace(property.Value.ToString(), @"{datetime}",
                    DateTime.UtcNow.ToUniversalTime().ToString("O"),
                    RegexOptions.IgnoreCase);
            }
        }
    }

    public class RequestReplaceProcessor : IProcessor
    {
        public void Process(IDictionary<string, object> result, KeyValuePair<string, object> property,
            IDictionary<string, object> request)
        {
            if (Regex.IsMatch(property.Value.ToString(), @"\[[\w\._]+\]", RegexOptions.IgnoreCase))
            {
                var propertyPath = Regex.Match(property.Value.ToString(), @"\[(?<propertyPath>[\w\._]+)\]",
                        RegexOptions.IgnoreCase)
                    .Groups["propertyPath"].Value.Split('.').ToList();

                string leaf = propertyPath.Last();
                propertyPath.Remove(leaf);

                var obj = request;
                foreach (var path in propertyPath)
                {
                    if (obj.TryGetValue(path, out var pathProperty) == false)
                        throw new InvalidOperationException($"Invalid path {property.Value}.");

                    if (pathProperty is JObject jsonObject)
                    {
                        obj = jsonObject.ToObject<IDictionary<string, object>>();
                    }
                }

                result[property.Key] = Regex.Replace(property.Value.ToString(), @"\[[\w\._]+\]", obj[leaf].ToString(),
                    RegexOptions.IgnoreCase);
            }
        }
    }
}