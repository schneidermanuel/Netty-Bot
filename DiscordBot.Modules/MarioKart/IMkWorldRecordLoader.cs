using System.Threading.Tasks;

namespace DiscordBot.Modules.MarioKart;

internal interface IMkWorldRecordLoader
{
    Task<WorldRecordData> LoadWorldRecord(string shortform, int cc);
}