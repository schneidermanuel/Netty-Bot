using System.Security.Cryptography;
using System.Text;

namespace DiscordBot.PubSub.Backend;

public class YoutubePubSubSecret
{
    private static string _secret;

    public static string Secret
    {
        get
        {
            if (string.IsNullOrEmpty(_secret))
            {
                _secret = Guid.NewGuid().ToString();
            }

            return _secret;
        }
    }

    public static bool Check(string body, string signature)
    {
        using (var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(Secret)))
        {
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(body));
            var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            return signature.Equals(hash);
        }
    }
}