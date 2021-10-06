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
            IConnectionMultiplexer connection = ConnectionMultiplexer.Connect("127.0.0.1");

            //IConnectionMultiplexer connection = RedisConnectionFactory.Connection;
            IDatabase database = connection.GetDatabase(7);

            IWordService wordService = new FileWordService("female-names.txt");

            ICompletionService completionService = new RedisCompletionService(database);

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

        private static readonly Lazy<IConnectionMultiplexer> connection = new Lazy<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost"));

        public static IConnectionMultiplexer Connection => connection.Value;

    }
}

  