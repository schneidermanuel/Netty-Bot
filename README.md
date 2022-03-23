# DiscordBotBeta

> Note: This Bot is currently in Beta state. Bugs may occur. 

## Requirements

To use this Bot, you first need to create an application [At the Discord API Portal](https://discord.com/developers/applications) and create an application. Make sure to enable '''Privileged Gateway Intents''' for your bot. 

You need a Mysql Database. A init script for the Database will be added soon, till then, create all the neccessary table from hand by checking out the mapping files for the entities located at ```DiscordBot.DataAccess/Entities```

Finally, you need a Lavalink Server in order to use the Music Player Module. 
Set the Port an IP Adress for this Server in ```Victoria/VictoriaModule.cs```

## Use the Bot

Once you saticfied all the requirements, rename the config_example.xml to config.xml located in DiscordBot.MainBot. Add the connection string to your database and the bot token for your discord bot where noted and remove the XML comment. 
Rename the hibernate_example.cfg.xml to hibernate.cfg.xml and pase in your sql connection string. 

## Running the bot

The bot can be run with the ```dotnet run``` command. 
To skip all the daily actions, that run every day for the current day, please use the ```dotnet run SkipDaily``` command. 

### Modules

> For each functionality of the bot, there should be one folder located in DiscordBot.Modules with a Module.cs, which inherits from Autofac.Module and  registers all the types. 
> A more detailed explanation of how to create your own modules will be added soon. 

#### Current Modules

##### AutoRole

> Automatically assigns a role to every user joining the server. 

###### Commands: 
- autoRoleConfig list 
> Prints out all the roles on the server that will be automatically assigned to new users. 
- autoRoleConfig add <RoleID>
> Sets a role up, that will be automatically distributed to new users. 
- autoRoleConfig delete <RoleId>
> Stops distributing a role
  
##### BirthdayList

> Creates a List of all the members on the server with their birthdays. Optionally notifies the server on a birthday and assigns a role to everyone at their birthday. 
> ATTENTION: This will stop working on BIIIG server, because of the message char limit. Will fix later. 
  
###### Commands
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
  
##### Huebcraft
  
> Private stuff - will be deleted when not used anymore. Just dont use it.
  
##### MusicPlayer
 
> Plays Music in your VC
  
###### Commands
- play <NAME>
- pause
- resume
- queue
- createPlaylist <NAME>
- deletePlaylist <ID>
- playlists
- playlist <ID>
- stop
  
##### ReactionRoles

> Assignes a role to everyone reacing to a message
  
###### Commands

- registerReactionRole <Emote> <RoleID> <Message>
> Writes <Message> in the Channel, reacted with <Emote> and distributes the <Role> when anyone reacts.
  
##### ZenQuote
  
> Sends a Quote in a channel every day. 
  
###### Commands
  
- registerQuote
- unregisterQuote
