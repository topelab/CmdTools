using System.Collections.Generic;

namespace SplitPDFWin.Extensions
{
    public static class CollectionsExtensions
    {
        public static void ClearAndAddRange<T>(this IList<T> collection, IEnumerable<T> values)
        {
            collection.Clear();
            collection.AddRange(values);
        }

        public static void AddRange<T>(this IList<T> collection, IEnumerable<T> values)
        {
            foreach (var value in values)
            {
                collection.Add(value);
            }
        }

        public static void RemoveRange<T>(this IList<T> collection, IEnumerable<T> values)
        {
            foreach (var value in values)
            {
                collection.Remove(value);
            }
        }
    }
}
