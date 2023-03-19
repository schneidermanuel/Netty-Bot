namespace DiscordBot.Modules.MarioKart;

internal class WorldRecordData
{
    public string Date { get; }
    public string Time { get; }
    public string Player { get; }
    public string Nation { get; }
    public string Lap1 { get; }
    public string Lap2 { get; }
    public string Lap3 { get; }
    public string Character { get; }
    public string Kart { get; }
    public string Tires { get; }
    public string Gilder { get; }
    public string Trackname { get; }
    public string VideoUrl { get; }

    public WorldRecordData(string date,
        string time,
        string player,
        string nation,
        string lap1,
        string lap2,
        string lap3,
        string character,
        string kart,
        string tires,
        string gilder,
        string trackname, 
        string videoUrl)
    {
        Date = date;
        Time = time;
        Player = player;
        Nation = nation;
        Lap1 = lap1;
        Lap2 = lap2;
        Lap3 = lap3;
        Character = character;
        Kart = kart;
        Tires = tires;
        Gilder = gilder;
        Trackname = trackname;
        VideoUrl = videoUrl;
    }
}