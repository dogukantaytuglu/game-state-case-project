using System.Collections.Generic;

public static class DictionaryExtensions
{
    public static void AddItemToCollectionValue<T1, T2, T3>(this Dictionary<T1, T2> dictionary,T1 key, T3 value) where T2 : ICollection<T3>, new()
    {
        if (dictionary.TryGetValue(key, out var listeners))
        {
            listeners.Add(value);
        }

        else
        {
            dictionary[key] = new T2 { value };
        }
    }
}