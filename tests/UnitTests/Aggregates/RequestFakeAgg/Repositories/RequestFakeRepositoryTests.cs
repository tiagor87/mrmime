using FluentAssertions;
using MrMime.Core.Aggregates.RequestFakeAgg.Repositories;
using Xunit;

namespace MrMime.UnitTests.Aggregates.RequestFakeAgg.Repositories
{
    public class RequestFakeRepositoryTests
    {
        [Theory]
        [InlineData("customers", "Get")]
        [InlineData("customers", "Post")]
        [InlineData("customers/1", "Put")]
        [InlineData("customers/1", "Delete")]
        public void Should_get_request_by_path_and_method(string path, string method)
        {
            var repository = RequestFakeRepository.Load("./db-test.json");

            var request = repository.GetRequestFake(path, method);

            request.Should().NotBeNull();
            request.Method.ToLower().Should().Be(method.ToLower());
        }

        [Fact]
        public void Should_load_json()
        {
            var repository = RequestFakeRepository.Load("./db-test.json");
            repository.Should().NotBeNull();
        }
    }
}