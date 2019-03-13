using System;
using System.Collections.Generic;
using System.Linq;
using MrMime.Core.Aggregates.RequestFakeAgg.Processers;
using Newtonsoft.Json.Linq;

namespace MrMime.Core.Aggregates.RequestFakeAgg.Builders
{
    public abstract class ResponseBuilder<TBuilder> where TBuilder : ResponseBuilder<TBuilder>
    {
        private IReadOnlyList<IProcessor> _processors;

        protected ResponseBuilder()
        {
            _processors = GetProcessors().ToList();
        }

        public IDictionary<string, object> RequestBody { get; private set; }

        public TBuilder FromRequest(IDictionary<string, object> requestBody)
        {
            RequestBody = requestBody ?? throw new ArgumentNullException(nameof(requestBody));
            return (TBuilder) this;
        }

        public abstract IDictionary<string, object> Build();

        protected IDictionary<string, object> ProcessResponse(IDictionary<string, object> response)
        {
            var result = new Dictionary<string, object>();
            foreach (var property in response)
            {
                if (property.Value != null && property.Value is JObject obj)
                {
                    var innerProperty = obj.ToObject<IDictionary<string, object>>();
                    result.Add(property.Key, ProcessResponse(innerProperty));
                    continue;
                }

                foreach (var processor in _processors.Where(x => x.ShouldExecute(property)))
                {
                    processor.Execute(result, property, RequestBody);
                }

                if (_processors.Any(x => x.ShouldExecute(property)) == false)
                {
                    result[property.Key] = property.Value;
                }
            }

            return result;
        }

        private static IEnumerable<IProcessor> GetProcessors()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            foreach (var typeInfo in assembly.DefinedTypes)
            {
                if (typeInfo.ImplementedInterfaces.Contains(typeof(IProcessor)) &&
                    string.IsNullOrWhiteSpace(typeInfo.FullName) == false)
                {
                    yield return assembly.CreateInstance(typeInfo.FullName) as IProcessor;
                }
            }
        }
    }
}