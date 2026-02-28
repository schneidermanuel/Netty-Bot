FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /opt/netty

COPY . .

RUN dotnet restore
RUN dotnet build DiscordBot.MainBot/DiscordBot.MainBot.csproj -c Release -o /app/build

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base

RUN apt-get update && apt-get install -y \
    curl \
    wget \
    libgtk-3-0 libdbus-glib-1-2 libxt6 \
    && rm -rf /var/lib/apt/lists/*

# Firefox installieren
RUN wget -qO- https://ftp.mozilla.org/pub/firefox/releases/117.0/linux-x86_64/en-US/firefox-117.0.tar.bz2 \
    | tar xjf - -C /opt/ && \
    ln -s /opt/firefox/firefox /usr/bin/firefox

WORKDIR /app

FROM base AS runtime
COPY --from=build /app/build .

CMD ["dotnet", "DiscordBot.MainBot.dll"]
