using Discord;

namespace DiscordBot.DataAccess.Contract.ReactionRoles
{
    public class ReactionRole
    {
        public long Id { get; set; }
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public ulong MessageId { get; set; }
        public IEmote Emote { get; set; }
        public ulong RoleId { get; set; }

    }
}