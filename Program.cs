using System;
using System.Collections.Generic;
using System.IO;
using StackExchange.Redis;
using System.Linq;
using static System.Math;

namespace redis_autocomplete
{
    // Auto Complete with Redis
    // Inspired by http://oldblog.antirez.com/post/autocomplete-with-redis.html
    class Program
    {
        const string key = "completion";

        // dotnet add package StackExchange.Redis
        static void Main(string[] args)
        {
        
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            IDatabase db = redis.GetDatabase();

            // string[] words =  { "bar", "foo", "foobar" };

            string[] words =  File.ReadAllLines("female-names.txt");

            if (!db.KeyExists(key))
            {
                Console.WriteLine("Loading entries to the Redis DB...");

                Create(db, words);
            }
            else
            {
                Console.WriteLine($"NOT loading entries, there is already a '{key}' key");
            }
            
            string prefix;
            do
            {
                System.Console.Write("Type prefix: ");
                prefix = Console.ReadLine(); 

                var autocompleteWords = Complete(db, prefix);

                foreach(var word in autocompleteWords)
                {
                    System.Console.WriteLine(word);
                }

            } 
            while(prefix!=string.Empty);
            
        }

        static void Create(IDatabase db, string[] words)
        {
            foreach(string word in words)
            {
                for(int l=1; l < word.Length; l++)
                {
                    string prefix = word.Substring(0, l);
                    // ZADD key prefix
                    db.SortedSetAdd(key, member: prefix, score: 0);
                }

                // ZADD key foo*
                db.SortedSetAdd(key, member: $"{word}*", score: 0);
            }
        }

        static string[] Complete(IDatabase db, string prefix)
        {
            const int rangelen = 50; // This is not random, try to get replies < MTU size

            IList<string> results = new List<string>();

            // ZRANK key fo
            long? start = db.SortedSetRank(key, prefix);

            if (!start.HasValue)
            {
                return results.ToArray();
            }
 
            // ZRANGE key 6 -1
            var range = db.SortedSetRangeByRank(key, start.Value, start.Value + rangelen - 1);
            start += rangelen;

            foreach(string entry in range)
            {
                int minlen = Min(entry.Length, prefix.Length);

                if (entry.Substring(0, minlen) != prefix.Substring(0, minlen))
                {
                    break;
                }

                if (entry.EndsWith("*"))
                {
                    string word = entry.Substring(0, entry.Length-1);
                    results.Add(word);
                }    
            }

            return results.ToArray();
        }
    }

   
}
