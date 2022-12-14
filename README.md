# Netty


## Donation

This Bot is completly free available for everyone to use. A fully public hosted Bot, including a web-based configuration frontend is available at [netty-bot.com](https://netty-bot.com)
Any donation is highly appreciated and helps keeping this project alive.
You may donate [here](https://tipeeestream.com/brainyxs/donation)

## Add the Bot

A fully hosted bot is available [here](https://invite.netty-bot.com). A Web configuration UI is planed.

### Self-Host this Bot

> If you wish, you may host this bot on your own. All the needed code is available.

### Requirements

To use this bot, you first need to create an application over [at the Discord API Portal](https://discord.com/developers/applications) and create an application. Make sure to enable ```Privileged Gateway Intents``` for your bot. 

You will need a Mysql Database. A init script for the database will be added soon, till then, create all the neccessary table from hand by checking the mapping files for the entities located at ```DiscordBot.DataAccess/Entities```

You must provide a hostname & port with port-forwarding configured to the system with the bot running for Youtube to send data to the rest api hosted in this project.

Rename the ```config_example.xml``` located in the ```DiscordBot.MainBot```to ```config.xml``` and add all the keys for your bot. 
This requires:

- Discord bot Token
- Twitch API Client ID & Client Secret
- Hostname & Port for the server the bot is running on. This will host a REST API for Youtube to send data to, once a video is uploaded by a followed channel.
- Lavalink Host & Port & Password & SSL Mode: You can either self-host your lavalink server or choose one of the many publicly available servers
- Youtube API Key with Youtube Data V3 enabled

Last, rename the hibernate_example.cfg.xml to hibernate.cfg.xml and pase in your sql connection string. 

### Running the bot

The bot can be run with the ```dotnet run``` command. 
To skip all the daily actions, that run every day for the current day, please use the ```dotnet run SkipDaily``` command.
This is recommended when testing the bot multiple times a day to not trigger those actions multiple times.  

#### Extending the bot

> For each feature of the bot, there should be one folder located in DiscordBot.Modules with a Module.cs, which inherits from Autofac.Module and  registers all the types. 
> A more detailed explanation of how to create your own modules will be added soon. 

You may create pull requests.

## Current Modules

### AutoRole

> Automatically assigns a role to every user joining the server. 

#### Commands: 
- autoRoleConfig list 
> Prints out all the roles on the server that will be automatically assigned to new users. 
- autoRoleConfig add <RoleID>
> Sets a role up, that will be automatically distributed to new users. 
- autoRoleConfig delete <RoleId>
> Stops distributing a role
  
### BirthdayList

> Creates a List of all the members on the server with their birthdays. Optionally notifies the server on a birthday and assigns a role to everyone at their birthday. 
> ATTENTION: This will stop working on BIIIG server, because of the message character limit. Will fix later. 
  
#### Commands
- registerBirthday <dd.MM>
> Adds your birthday to the database
- registerBirthdayChannel
> Prints and automatically updates the birthdays in this channel
- subBirthdays
> At a birthday of a user, there will be a notification in this channel.
- unsubBirthdays
> What do you think this does?
- setBirthdayRole <RoleID>
> Automatically distributes a role at a birhday
  
  
### MusicPlayer
 
> Plays music in a voice chat with the ability to create and use playlists.
  
#### Commands
- play <NAME>
- pause
- resume
- queue
- createPlaylist <NAME>
- deletePlaylist <ID>
- playlists
- playlist <ID>
- stop
  
### ReactionRoles

> Assignes a role to everyone reacing to a message
  
#### Commands

- registerReactionRole <Emote> <RoleID> <Message>
> Sends <Message> in the channel, reactes with <Emote> and distributes the <Role> to anyone reacting.
  
### ZenQuote
  
> Sends a quote in a channel once a day. 
  
#### Commands
  
- registerQuote
- 

### TwitchNotifications

> Sends a notification in a channel when a twitch user starts a live stream

#### Commands

- registerTwitch <twitchUsername> <message>
> Registers the current channel for notifications for the <twitchUsername>. The <message> paramter is optional.
- unregisterTwitch <twitchUsername>
> Removes a registration.

### YoutubeNotifications

> Sends a notification in a channel when a Youtube channel uploads a video

#### Commands

- registerYoutube <channelId> <message>
> Registers the current channel for notifications for the <channelId>. The <message> paramter is optional.
- unregisterYoutube <channelId>
> Removes a registration.
