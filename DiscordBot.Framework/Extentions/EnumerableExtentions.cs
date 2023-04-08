using System.Collections.Generic;
using System.Linq;

namespace DiscordBot.Framework.Extentions;

public static class EnumerableExtentions
{
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> values)
    {
        return values.Where(value => value != null);
    }

    public static void AddRange<T>(this IList<T> list, IEnumerable<T> toAdd)
    {
        foreach (var val in toAdd)
        {
            list.Add(val);
        }
    }

    public static void AddRange<TKey, TVal>(this IDictionary<TKey, TVal> list, IDictionary<TKey, TVal> toAdd)
    {
        foreach (var val in toAdd)
        {
            list.Add(val.Key, val.Value);
        }
    }
}