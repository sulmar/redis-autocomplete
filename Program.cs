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
        // dotnet add package StackExchange.Redis
        static void Main(string[] args)
        {
            // ConnectionMultiplexer connection = ConnectionMultiplexer.Connect("localhost");

            var connection = RedisConnectionFactory.Connection;
            IDatabase database = connection.GetDatabase();

            IWordService wordService = new FileWordService("female-names.txt");

            ICompletionService completionService = new RedisCompletionService(database);

            if (!completionService.Exists)
            {
                Console.WriteLine("Loading entries...");

                IEnumerable<string> words = wordService.Get();

                completionService.AddRange(words);
            }

            string prefix;

            do
            {
                Console.Write("Type prefix: ");

                prefix = Console.ReadLine();

                var autocompleteWords = completionService.Get(prefix);

                foreach (var word in autocompleteWords)
                {
                    Console.WriteLine(word);
                }

            }
            while (prefix != string.Empty);

        }
    }

    public class RedisConnectionFactory
    {

        private static readonly Lazy<ConnectionMultiplexer> connection = new Lazy<ConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost"));

        public static ConnectionMultiplexer Connection => connection.Value;

    }
}

  