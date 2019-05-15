using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using MrMime.Core.Aggregates.RequestFakeAgg.Repositories;
using Newtonsoft.Json.Linq;

namespace MrMime.Api.Controllers
{
    [Route("{*path}")]
    [ApiController]
    public class DefaultController : ControllerBase
    {
        private readonly RequestFakeRepository _repository;

        public DefaultController(RequestFakeRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        /// <summary>
        ///     Fakes a get and return Ok (200) with the path
        /// </summary>
        /// <param name="path">Path called</param>
        /// <param name="query">Query parameters</param>
        /// <returns>
        ///     Ok with
        ///     <param name="path"></param>
        /// </returns>
        [HttpGet]
        public IActionResult Get(string path, [FromQuery] IDictionary<string, string> query)
        {
            var fake = _repository.GetRequestFake(path, HttpMethod.Get.Method);
            if (fake.IsStreamResponse)
                return File(fake.GetStreamResponse(), MediaTypeNames.Application.Octet);
            return StatusCode(fake.GetResponseStatusCode(), fake.GetResponse(JObject.FromObject(query), path));
        }

        /// <summary>
        ///     Fakes a post and return Created (201) with body content
        /// </summary>
        /// <param name="path">Path called</param>
        /// <param name="value">Body content</param>
        /// <returns>
        ///     Created with
        ///     <param name="value">body content</param>
        /// </returns>
        [HttpPost]
        public IActionResult Post(string path, [FromBody] JObject value)
        {
            var fake = _repository.GetRequestFake(path, HttpMethod.Post.Method);
            if (fake.IsStreamResponse)
                return File(fake.GetStreamResponse(), MediaTypeNames.Application.Octet);
            return StatusCode(fake.GetResponseStatusCode(), fake.GetResponse(value, path));
        }

        /// <summary>
        ///     Fakes an update and return Ok (200) with body content
        /// </summary>
        /// <param name="path">Path called</param>
        /// <param name="value">Body content</param>
        /// <returns>
        ///     Ok with
        ///     <param name="value">body content</param>
        /// </returns>
        [HttpPut]
        public IActionResult Put(string path, [FromBody] JObject value)
        {
            var fake = _repository.GetRequestFake(path, HttpMethod.Put.Method);
            return StatusCode(fake.GetResponseStatusCode(), fake.GetResponse(value, path));
        }

        /// <summary>
        ///     Fakes a delete and returns No Content (204) status code
        /// </summary>
        /// <param name="path">Path called</param>
        /// <returns>No content</returns>
        [HttpDelete]
        public IActionResult Delete(string path)
        {
            var fake = _repository.GetRequestFake(path, HttpMethod.Delete.Method);
            return StatusCode(fake.GetResponseStatusCode());
        }
    }
}