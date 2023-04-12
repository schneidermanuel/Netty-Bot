using System.Collections.Generic;
using System.Linq;

namespace DiscordBot.Modules.MarioKart;

internal class MkWorldRecordMapper : IMkWorldRecordMapper
{
    public string GetNameByShortForm(string shortform)
    {
        var pair = MarioKartMapProvider.Maps.SingleOrDefault(map => map.Key.ToLower() == shortform.ToLower());
        return pair.Equals(default(KeyValuePair<string, string>)) ? null : pair.Value;
    }
}