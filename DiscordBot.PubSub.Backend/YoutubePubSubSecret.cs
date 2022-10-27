using System.Security.Cryptography;
using System.Text;

namespace DiscordBot.PubSub.Backend;

public class YoutubePubSubSecret
{
    public static string Secret { get; } = Guid.NewGuid().ToString();

    public static bool Check(string hash)
    {
        using var sha1 = SHA1.Create();
        var correctHash = Convert.ToHexString(sha1.ComputeHash(Encoding.UTF8.GetBytes(Secret)));
        return hash.Equals(correctHash);
    }
}