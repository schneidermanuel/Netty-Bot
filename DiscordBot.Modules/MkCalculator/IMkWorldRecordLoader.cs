using System.Threading.Tasks;

namespace DiscordBot.Modules.MkCalculator;

internal interface IMkWorldRecordLoader
{
    Task<WorldRecordData> LoadWorldRecord(string shortform, int cc);
}