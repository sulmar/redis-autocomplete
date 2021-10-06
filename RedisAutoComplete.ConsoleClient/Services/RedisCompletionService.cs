using StackExchange.Redis;
using System;
using System.Collections.Generic;
using static System.Math;

namespace redis_autocomplete
{
    public class RedisCompletionService : ICompletionService
    {
        private readonly IDatabase db;
        private const string key = "completion";

        public RedisCompletionService(IDatabase db)
        {
            this.db = db;
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
            const int rangelen = 50; // This is not random, try to get replies < MTU size

            IList<string> results = new List<string>();

            // ZRANK key fo
            long? start = db.SortedSetRank(key, prefix);

            if (!start.HasValue)
            {
                return results;
            }

            // ZRANGE key 6 -1
            var range = db.SortedSetRangeByRank(key, start.Value, start.Value + rangelen - 1);
            start += rangelen;

            foreach (string entry in range)
            {
                int minlen = Min(entry.Length, prefix.Length);

                if (entry.Substring(0, minlen) != prefix.Substring(0, minlen))
                {
                    break;
                }

                if (entry.EndsWith("*"))
                {
                    string word = entry.Substring(0, entry.Length - 1);
                    results.Add(word);
                }
            }

            return results;
        }
    }
}
