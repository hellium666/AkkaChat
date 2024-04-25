namespace AkkaChat.Server.Utils;

internal static class DictionaryExtensions
{
    public static HashSet<string> GetOrAdd(this IDictionary<string, HashSet<string>> dict, string key, ref long counter)
    {
        if (dict.TryGetValue(key, out var value))
            return value;

        Interlocked.Increment(ref counter);
        var newValue = new HashSet<string>();
        dict.Add(key, newValue);
        return newValue;
    }
    
    public static HashSet<string> GetOrAdd(this IDictionary<string, HashSet<string>> dict, string key)
    {
        if (dict.TryGetValue(key, out var value))
            return value;

        var newValue = new HashSet<string>();
        dict.Add(key, newValue);
        return newValue;
    }
}