using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using MrMime.Core.Aggregates.RequestFakeAgg.Processers;
using Newtonsoft.Json.Linq;

namespace MrMime.Core.Aggregates.RequestFakeAgg.Builders
{
    public abstract class ResponseBuilder<TBuilder> where TBuilder : ResponseBuilder<TBuilder>
    {
        private readonly IReadOnlyList<IProcessor> _processors;

        protected ResponseBuilder()
        {
            _processors = GetProcessors().ToList();
        }

        public JObject RequestBody { get; private set; }
        public IDictionary<string, string> UrlParameters { get; private set; }

        public TBuilder FromRequest(JObject requestBody)
        {
            RequestBody = requestBody ?? throw new ArgumentNullException(nameof(requestBody));
            return (TBuilder) this;
        }

        public TBuilder WithUrlParams(IDictionary<string, string> parameters)
        {
            UrlParameters = new ReadOnlyDictionary<string, string>(parameters);
            return (TBuilder) this;
        }

        public abstract JObject Build();

        protected JObject ProcessResponse(JObject response)
        {
            var result = new JObject();
            foreach (var property in response)
            {
                if (property.Value != null && property.Value is JArray list)
                {
                    var propertyList = new List<JObject>();
                    foreach (var item in list.ToList()) propertyList.Add(ProcessResponse(item.ToObject<JObject>()));
                    result.Add(property.Key, JArray.FromObject(propertyList));
                    continue;
                }

                if (property.Value != null && property.Value is JObject obj)
                {
                    var innerProperty = obj;
                    result.Add(property.Key, ProcessResponse(innerProperty));
                    continue;
                }

                foreach (var processor in _processors.Where(x => x.ShouldExecute(property)))
                    processor.Execute(result, property, RequestBody, UrlParameters);

                if (_processors.Any(x => x.ShouldExecute(property)) == false) result[property.Key] = property.Value;
            }

            return result;
        }

        private static IEnumerable<IProcessor> GetProcessors()
        {
            var assembly = Assembly.GetExecutingAssembly();

            foreach (var typeInfo in assembly.DefinedTypes)
                if (typeInfo.ImplementedInterfaces.Contains(typeof(IProcessor)) &&
                    string.IsNullOrWhiteSpace(typeInfo.FullName) == false)
                    yield return assembly.CreateInstance(typeInfo.FullName) as IProcessor;
        }
    }
}