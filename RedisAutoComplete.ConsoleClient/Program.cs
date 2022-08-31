using System;
using System.Collections.Generic;
using StackExchange.Redis;

namespace redis_autocomplete
{
    // Auto Complete with Redis
    // Inspired by http://oldblog.antirez.com/post/autocomplete-with-redis.html
    class Program
    {
        // dotnet add package StackExchange.Redis
        static void Main(string[] args)
        {            
            const string host = "127.0.0.1";

            Console.WriteLine($"Connecting to {host}...");
            IConnectionMultiplexer connection = ConnectionMultiplexer.Connect(host);

            Console.WriteLine("Connected.");

            // IWordService wordService = new FileWordService("female-names.txt");
            IWordService wordService = new FileWordService("cities.txt");

            ICompletionService completionService = new RedisCompletionService(connection);

            if (!completionService.Exists)
            {
                Console.WriteLine("Loading entries...");

                IEnumerable<string> words = wordService.Get();

                IProgress<string> progress = new Progress<string>(word => Console.Write($",{word}"));

                completionService.AddRange(words, progress);
            }


            Console.WriteLine("----------------");


            string prefix;

            do
            {
                Console.Write("Type prefix: ");

                prefix = Console.ReadLine();

                var autocompleteWords = completionService.Get(prefix);

                int index = 1;
                foreach (var word in autocompleteWords)
                {
                    Console.WriteLine($"({index++}) {word}");
                }

            }
            while (prefix != string.Empty);

        }
    }

    public class RedisConnectionFactory
    {

        private static readonly Lazy<IConnectionMultiplexer> connection = new Lazy<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost"));

        public static IConnectionMultiplexer Connection => connection.Value;

    }
}

  