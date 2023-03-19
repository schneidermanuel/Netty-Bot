using System.Collections.Generic;
using System.Linq;

namespace DiscordBot.Framework.Extentions;

public static class EnumerableExtentions
{
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> values)
    {
        return values.Where(value => value != null);
    }
}