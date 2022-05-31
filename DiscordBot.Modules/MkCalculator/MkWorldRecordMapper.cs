using System.Collections.Generic;

namespace DiscordBot.Modules.MkCalculator;

internal class MkWorldRecordMapper : IMkWorldRecordMapper
{
    private static Dictionary<string, string> _map = new Dictionary<string, string>
    {
        { "MKS", "Mario+Kart+Stadium" },
        { "WP", "Water+Park" },
        { "SSC", "Sweet+Sweet+Canyon" },
        { "TR", "Thwomp+Ruins" },
        { "MC", "Mario+Circuit" },
        { "TH", "Toad+Harbor" },
        { "TM", "Twisted+Mansion" },
        { "SGF", "Shy+Guy+Falls" },
        { "SA", "Sunshine+Airport" },
        { "DS", "Dolphin+Shoals" },
        { "Ed", "Electrodrome" },
        { "MW", "Mount+Wario" },
        { "CC", "Cloudtop+Cruise" },
        { "BDD", "Bone-Dry+Dunes" },
        { "BS", "Bowser%27s+Castle" },
        { "RR", "Rainbow+Road" },
        { "rMMM", "Wii+Moo+Moo+Meadows" },
        { "rMC", "GBA+Mario+Circuit" },
        { "rCCB", "DS+Cheep+Cheep+Beach" },
        { "rTT", "N64+Toad%27s+Turnpike" },
        { "rDDD", "GCN+Dry+Dry+Desert" },
        { "rDP3", "SNES+Donut+Plains+3" },
        { "rRRy", "N64+Royal+Raceway" },
        { "rDKJ", "3DS+DK+Jungle" },
        { "rWS", "DS+Wario+Stadium" },
        { "rSL", "GCN+Sherbet+Land" },
        { "rMP", "3DS+Music+Park" },
        { "rYV", "N64+Yoshi+Valley" },
        { "rTTC", "DS+Tick-Tock+Clock" },
        { "rPPS", "Piranha+Plant+Slide" },
        { "rGV", "Wii+Grumble+Volcano" },
        { "rRRd", "N64+Rainbow+Road" },
        { "dYC", "GCN+Yoshi+Circuit" },
        { "dWS", "DS+Wario+Stadium" },
        { "dEA", "Excitebike+Arena" },
        { "dDD", "Dragon+Driftway" },
        { "dMC", "Mute+City" },
        { "dWGM", "Wii+Wario%27s+Gold+Mine" },
        { "dRR", "SNES+Rainbow+Road" },
        { "dIIO", "Ice+Ice+Outpost" },
        { "dHC", "Hyrule+Circuit" },
        { "dBP", "GCN+Baby+Park" },
        { "dCL", "GBA+Cheese+Land" },
        { "dWW", "Wild+Woods" },
        { "dAC", "Animal+Crossing" },
        { "dNBC", "3DS+Neo+Bowser+City" },
        { "dRiR", "GBA+Ribbon+Road" },
        { "dSBS", "Super+Bell+Subway" },
        { "dBB", "Big+Blue" },
        { "bPP", "Tour+Paris+Promenade" },
        { "bTC", "3DS+Toad+Circuit" },
        { "bCMo", "N64+Choco+Mountain" },
        { "bCMa", "Wii+Coconut+Mall" },
        { "bTB", "Tour+Tokyo+Blur" },
        { "bSR", "DS+Shroom+Ridge" },
        { "bSG", "GBA+Sky+Garden" },
        { "bNH", "Tour+Ninja+Hideaway" },
    };

    public string GetNameByShortForm(string shortform)
    {
        if (!_map.ContainsKey(shortform))
        {
            return null;
        }

        return _map[shortform];
    }
}