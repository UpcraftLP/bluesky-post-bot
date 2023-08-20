FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Up.Bsky.PostBot.csproj", "./"]
RUN dotnet restore "Up.Bsky.PostBot.csproj"
COPY . .
RUN dotnet build "Up.Bsky.PostBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Up.Bsky.PostBot.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Up.Bsky.PostBot.dll"]
