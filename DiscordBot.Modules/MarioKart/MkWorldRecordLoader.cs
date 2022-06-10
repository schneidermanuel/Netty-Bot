using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace DiscordBot.Modules.MarioKart;

internal class MkWorldRecordLoader : IMkWorldRecordLoader
{
    private readonly IMkWorldRecordMapper _recordMapper;
    private static readonly int[] AvailableCc = { 150, 200 };

    public MkWorldRecordLoader(IMkWorldRecordMapper recordMapper)
    {
        _recordMapper = recordMapper;
    }


    public async Task<WorldRecordData> LoadWorldRecord(string shortform, int cc)
    {
        if (!AvailableCc.Contains(cc))
        {
            return null;
        }

        var trackname = _recordMapper.GetNameByShortForm(shortform);
        return string.IsNullOrEmpty(trackname) ? null : await LoadData(trackname, cc);
    }

    private async Task<WorldRecordData> LoadData(string trackname, int cc)
    {
        var url = $@"https://mkwrs.com/mk8dx/display.php?track={trackname}&m={cc}";
        var client = new HtmlWeb();

        try
        {
            var website = await client.LoadFromWebAsync(url);
            var infos = website.DocumentNode
                .SelectNodes("//table[contains(concat(' ', normalize-space(@class), ' '), ' wr ')]").First()
                .SelectNodes("tr").Skip(1).First().SelectNodes("td").ToArray();
            var date = infos.First().InnerText;
            var time = infos.Skip(1).First().InnerText;
            var player = infos.Skip(2).First().InnerText;
            var nation = @"https://mkwrs.com/country-flags/" +
                         Regex.Replace(
                             Regex.Replace(infos.Skip(3).First().InnerHtml, ".*src=\"..\\/country-flags\\/", ""),
                             "\">.*", "");
            var lap1 = infos.Skip(5).First().InnerText;
            var lap2 = infos.Skip(6).First().InnerText;
            var lap3 = infos.Skip(7).First().InnerText;
            var character = infos.Skip(10).First().InnerText;
            var kart = infos.Skip(11).First().InnerText;
            var tires = infos.Skip(12).First().InnerText;
            var gilder = infos.Skip(13).First().InnerText;
            string videoUrl;
            try
            {
                videoUrl = Regex.Replace(Regex.Replace(infos.Skip(1).First().InnerHtml, "<.*href=\"", ""), "\".*", "");
            }
            catch (Exception)
            {
                videoUrl = null;
            }

            if (!string.IsNullOrEmpty(videoUrl) && !videoUrl.StartsWith("https://"))
            {
                videoUrl = null;
            }

            return new WorldRecordData(date, time, player, nation, lap1, lap2, lap3, character, kart, tires, gilder,
                trackname.Replace("+", " ").Replace("%27", "'"), videoUrl);
        }
        catch (Exception)
        {
            return null;
        }

        return null;
    }
}