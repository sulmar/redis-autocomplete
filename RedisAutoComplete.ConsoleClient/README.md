# Auto Complete with Redis in C#

Inspired by Auto Complete with Redis
http://oldblog.antirez.com/post/autocomplete-with-redis.html


## Create instance
~~~ csharp
IConnectionMultiplexer connection = ConnectionMultiplexer.Connect("localhost");
IDatabase database = connection.GetDatabase();
ICompletionService completionService = new RedisCompletionService(database);
~~~

## Load words to dictionary
~~~ csharp
string[] words = { "bar", "foo", "foobar" };
completionService.AddRange(words);
~~~

## Autocomplete 
~~~ csharp
var autocompleteWords = completionService.Get(prefix);

foreach (var word in autocompleteWords)
{
    Console.WriteLine(word);
}
~~~
