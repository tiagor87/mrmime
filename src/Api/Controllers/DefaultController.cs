using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using MrMime.Core.Aggregates.RequestFakeAgg.Repositories;

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
        /// <returns>Ok with
        ///     <param name="path"></param>
        /// </returns>
        [HttpGet]
        public IActionResult Get(string path, [FromQuery] IDictionary<string, string> query)
        {
            var dic = new Dictionary<string, object>();
            foreach (var pair in query) dic.Add(pair.Key, pair.Value);
            var fake = _repository.GetRequestFake(path, HttpMethod.Get.Method);
            return Ok(fake.GetResponse(dic));
        }

        /// <summary>
        ///     Fakes a post and return Created (201) with body content
        /// </summary>
        /// <param name="path">Path called</param>
        /// <param name="value">Body content</param>
        /// <returns>Created with
        ///     <param name="value">body content</param>
        /// </returns>
        [HttpPost]
        public IActionResult Post(string path, [FromBody] IDictionary<string, object> value)
        {
            var fake = _repository.GetRequestFake(path, HttpMethod.Post.Method);
            return StatusCode(201, fake.GetResponse(value));
        }

        /// <summary>
        ///     Fakes an update and return Ok (200) with body content
        /// </summary>
        /// <param name="path">Path called</param>
        /// <param name="value">Body content</param>
        /// <returns>Ok with
        ///     <param name="value">body content</param>
        /// </returns>
        [HttpPut]
        public IActionResult Put(string path, [FromBody] IDictionary<string, object> value)
        {
            var fake = _repository.GetRequestFake(path, HttpMethod.Put.Method);
            return Ok(fake.GetResponse(value));
        }

        /// <summary>
        ///     Fakes a delete and returns No Content (204) status code
        /// </summary>
        /// <param name="path">Path called</param>
        /// <returns>No content</returns>
        [HttpDelete]
        public IActionResult Delete(string path)
        {
            return NoContent();
        }
    }
}