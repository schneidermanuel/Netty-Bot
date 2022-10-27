using System.Security.Cryptography;
using System.Text;

namespace DiscordBot.PubSub.Backend;

public class YoutubePubSubSecret
{
    public static string Secret { get; } = Guid.NewGuid().ToString();

    public static bool Check(string body, string signature)
    {
        using (var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(Secret)))
        {
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(body));
            var hash = Encoding.UTF8.GetString(hashBytes);
            Console.WriteLine("Computed Hash " + hash);
            return signature.Equals(hash);
        }
    }
}