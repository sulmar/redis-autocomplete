using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace redis_autocomplete
{
    public class RedisCompletionService : ICompletionService
    {
        
        private readonly IConnectionMultiplexer connection;
        private const string key = "completion";
        private IDatabase db => connection.GetDatabase();

        private const int rangelen = 50; // This is not random, try to get replies < MTU size


        public RedisCompletionService(IConnectionMultiplexer connection)
        {            
            this.connection = connection;
        }

        public bool Exists => db.KeyExists(key);

        public void AddRange(IEnumerable<string> words, IProgress<string> progress = default)
        {
            foreach (string word in words)
            {
                Add(word);
                progress?.Report(word);
            }
        }

        public void Add(string word)
        {
            for (int l = 1; l < word.Length; l++)
            {
                string prefix = word.Substring(0, l);
                // ZADD key prefix
                db.SortedSetAdd(key, member: prefix, score: 0);
            }

            // ZADD key foo*
            db.SortedSetAdd(key, member: $"{word}*", score: 0);
        }

        public IEnumerable<string> Get(string prefix)
        {
            // ZRANK key fo
            long? start = db.SortedSetRank(key, prefix);

            if (start.HasValue)
            {
                // ZRANGE key 6 -1
                var range = db.SortedSetRangeByRank(key, start.Value, start.Value + rangelen - 1);

                var results = range
                    .Select(entry => entry.ToString())
                    .Where(entry => entry.StartsWith(prefix))
                    .Where(entry => entry.EndsWith("*"))
                    .Select(entry => entry.RemoveLast());

                return results;
            }
            else
                return Enumerable.Empty<string>();
        }
    }

    public static class StringExtensions
    {
        public static string RemoveLast(this string value) => value.Remove(value.Length - 1, 1);
    }
}
