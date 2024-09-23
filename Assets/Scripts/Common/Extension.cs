using System.Collections.Generic;

public static class StringExtension {
    public static string ToUpperFirstLetter(this string self) {
        if (string.IsNullOrEmpty(self))
        {
            return self;
        }
        return char.ToUpper(self[0]) + self[1..];
    }
}

public static class DictionaryExtension {
    public static List<TValue> GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, List<TValue>> self, TKey key) {
        if (self.TryGetValue(key, out var list))
            return list;
        list = new List<TValue>();
        self[key] = list;
        return list;
    }
}