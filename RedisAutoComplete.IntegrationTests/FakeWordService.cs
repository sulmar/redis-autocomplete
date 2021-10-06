using redis_autocomplete;
using System.Collections.Generic;

namespace RedisAutoComplete.IntegrationTests
{
    public class FakeWordService : IWordService
    {
        public IEnumerable<string> Get() => new string[] {
                                                            "aarika",
                                                            "abagael",
                                                            "aaren",
                                                            "aarika",
                                                            "abagael",
                                                            "abagail",
                                                            "abbe",
                                                            "abbey",
                                                            "abbi",
                                                            "abbie",
                                                            "abby",
                                                            "abbye",
            };
    }
}
