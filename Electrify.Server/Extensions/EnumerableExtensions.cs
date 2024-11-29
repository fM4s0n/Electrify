namespace Electrify.Server.Extensions;

public static class EnumerableExtensions
{
    public static T? MaxOrDefault<T>(this IEnumerable<T> source) where T : IComparable<T>
    {
        T? max = default;
        
        foreach (var t in source)
        {
            if (max is null)
            {
                max = t;
            }
            else
            {
                if (max.CompareTo(t) < 0)
                {
                    max = t;
                }
            }
        }

        return max;
    }
}