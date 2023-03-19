using System.Collections.Generic;

namespace DiscordBot.DataAccess.Contract.AutoMod;

public class AutoModRule
{
    public string RuleKey { get; set; }
    public IEnumerable<AutoModConfig> Configs { get; set; }
}