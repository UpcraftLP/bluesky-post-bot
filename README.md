# Bluesky Post Bot

A discord bot to listen for posts on bluesky and repost them in a discord channel


## How to use

1. Set up a [PostgreSQL](https://hub.docker.com/_/postgres) database to connect the API to.

2. Simply download and run the docker container:

    ```sh
    docker pull ghcr.io/upcraftlp/bluesky-post-bot:latest
    ```

    An [example appsettings file](appsettings.Example.json) is provided to list all configuration values. Alternatively those can be supplied via environment variables. See the [ASP.NET Core documentation](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-7.0) on configuration.



