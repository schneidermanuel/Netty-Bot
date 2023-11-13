ROM	alpine
RUN	apk add dotnet6-sdk git bash firefox ttf-dejavu ttf-liberation
WORKDIR	/opt/netty
RUN	git clone https://github.com/BrainyXS/Netty-Bot
WORKDIR /opt/netty/Netty-Bot
RUN	git checkout release
COPY	config.xml /opt/netty/Netty-Bot/DiscordBot.MainBot/config.xml
COPY	hibernate.cfg.xml /opt/netty/Netty-Bot/DiscordBot.DataAccess/hibernate.cfg.xml
RUN	dotnet build
CMD	["/bin/bash", "-c", "dotnet run --project /opt/netty/Netty-Bot/DiscordBot.MainBot/DiscordBot.MainBot.csproj SkipDaily"]
