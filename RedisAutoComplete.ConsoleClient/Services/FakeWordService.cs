using System.Collections.Generic;

namespace redis_autocomplete
{
    public class FakeWordService : IWordService
    {
        public IEnumerable<string> Get()
        {
            string[] words = { "bar", "foo", "foobar" };

            return words;
        }
    }
}
