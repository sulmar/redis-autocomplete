using Ductus.FluentDocker.Services;
using redis_autocomplete;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisAutoComplete.IntegrationTests
{
    public class RedisDatabaseFixture : IDisposable
    {
        public IDatabase Database { get; private set; }

        private IContainerService container;

        public ICompletionService completionService { get; private set; }

        public RedisDatabaseFixture()
        {
            container = new Ductus.FluentDocker.Builders.Builder()
             .UseContainer()
             .WithHostName("localhost")
             .UseImage("redis")
             .ExposePort(6379, 6379)
             .WaitForPort("6379/tcp", TimeSpan.FromSeconds(30))
             .Build()
             .Start();

            IConnectionMultiplexer connection = ConnectionMultiplexer.Connect("localhost");

            // Load sample data
            // IWordService wordService = new FakeWordService();
            IWordService wordService = new FileWordService("female-names.txt");

            completionService = new RedisCompletionService(connection);

            if (!completionService.Exists)
            {
                IEnumerable<string> words = wordService.Get();

                completionService.AddRange(words);
            }
        }

        public void Dispose()
        {                        
            container.Dispose();
        }
    }
}
