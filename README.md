# Przykład aplikacji z użyciem REDIS i .NET Core

### Uruchomienie Redis w dockerze

~~~
docker run --name autocomplete-redis -d -p 6379:6379 redis
~~~

Uruchomienie trybu interaktywnego
~~~
docker exec -it autocomplete-redis redis-cli
~~~
