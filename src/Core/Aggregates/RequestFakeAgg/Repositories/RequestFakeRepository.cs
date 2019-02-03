using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using MrMime.Core.Aggregates.RequestFakeAgg.Builders;
using Newtonsoft.Json;

namespace MrMime.Core.Aggregates.RequestFakeAgg.Repositories
{
    public class RequestFakeRepository
    {
        private readonly RequestFake _defaultRequest;
        private readonly IReadOnlyList<RequestFake> _requestFakes;

        public RequestFakeRepository(IReadOnlyList<RequestFake> requestFakes)
        {
            _requestFakes = requestFakes ?? throw new ArgumentNullException(nameof(requestFakes));
            _defaultRequest = new RequestFake
            {
                Method = "*",
                Path = "*",
                ResponseBuilderType = ResponseBuilderType.RequestReflect
            };
        }

        public RequestFake GetRequestFake(string path, string method)
        {
            return _requestFakes.FirstOrDefault(x =>
                       Regex.IsMatch(path, x.Path, RegexOptions.IgnoreCase) && x.Method?.ToLower() == method?.ToLower())
                   ?? _defaultRequest;
        }

        public static RequestFakeRepository Load(string path)
        {
            using (var streamReader = new StreamReader(path))
            {
                var fakes = JsonConvert.DeserializeObject<List<RequestFake>>(streamReader.ReadToEnd()).AsReadOnly();
                return new RequestFakeRepository(fakes);
            }
        }
    }
}