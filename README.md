# Auto Complete with Redis in C#

Inspired by Auto Complete with Redis
http://oldblog.antirez.com/post/autocomplete-with-redis.html


## Utworzenie us³ugi
~~~ csharp
ConnectionMultiplexer connection = ConnectionMultiplexer.Connect("localhost");
ICompletionService completionService = new RedisCompletionService(database);
~~~

## Za³adowanie s³ów do s³ownika
~~~ csharp
string[] words = { "bar", "foo", "foobar" };
completionService.AddRange(words);
~~~

## Wyszukiwanie 
~~~ csharp
var autocompleteWords = completionService.Get(prefix);

foreach (var word in autocompleteWords)
{
    Console.WriteLine(word);
}
~~~