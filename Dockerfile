FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /opt/netty

COPY . .

RUN dotnet restore

RUN dotnet build DiscordBot.MainBot/DiscordBot.MainBot.csproj -c Release -o /app/build

FROM mcr.microsoft.com/dotnet/aspnet:10.0

RUN apt-get update && \
    apt-get install -y firefox-esr && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/build .

CMD ["dotnet", "DiscordBot.MainBot.dll"]
