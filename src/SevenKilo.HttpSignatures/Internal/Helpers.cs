namespace SevenKilo.HttpSignatures.Internal;

internal static class Helpers
{
    internal static IEnumerable<IEnumerable<KeyValuePair<TKey, TValue>>> TraverseKeyValuePaths<TKey, TValue>
    (
        IEnumerable<KeyValuePair<TKey, TValue>> items,
        IEnumerable<TKey> keys
    ) where TKey : IComparable where TValue : IComparable
    {
        var itemList = items.ToList();
        var keyList = keys.ToList();
        var pathList = new List<List<KeyValuePair<TKey, TValue>>>([[]]);

        for (var keyIndex = 0; keyIndex < keyList.Count; keyIndex++)
        {
            var key = keyList[keyIndex];
            var valueList = itemList.Where(kv => kv.Key.Equals(key)).Select(kv => kv.Value).ToList();
            var oldPath = pathList.Last().ToList();

            for (var valueIndex = 0; valueIndex < valueList.Count; valueIndex++)
            {
                var value = valueList[valueIndex];

                if (valueIndex == 0)
                {
                    for (var pathIndex = 0; pathIndex < pathList.Count; pathIndex++)
                    {
                        pathList[pathIndex].Add(new(key, value));
                    }
                }
                else
                {
                    var newPath = new List<KeyValuePair<TKey, TValue>>(oldPath)
                    {
                        new(key, value)
                    };
                    pathList.Add(newPath);
                }
            }
        }

        return pathList;
    }
}
