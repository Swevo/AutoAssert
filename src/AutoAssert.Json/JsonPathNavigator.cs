using System.Text.Json;

namespace AutoAssert;

/// <summary>
/// Resolves a simple dotted/indexed path (e.g. <c>"customer.address.city"</c> or
/// <c>"items[0].id"</c>) against a <see cref="JsonElement"/>.
/// </summary>
internal static class JsonPathNavigator
{
    public static bool TryGetValue(JsonElement root, string path, out JsonElement value)
    {
        value = root;

        foreach (var token in Tokenize(path))
        {
            if (token.IsIndex)
            {
                if (value.ValueKind != JsonValueKind.Array || token.Index >= value.GetArrayLength())
                {
                    return false;
                }

                value = value[token.Index];
            }
            else
            {
                if (value.ValueKind != JsonValueKind.Object || !value.TryGetProperty(token.Name!, out var next))
                {
                    return false;
                }

                value = next;
            }
        }

        return true;
    }

    private static IEnumerable<PathToken> Tokenize(string path)
    {
        var segments = path.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var segment in segments)
        {
            var name = segment;
            var bracketIndex = name.IndexOf('[');

            if (bracketIndex < 0)
            {
                yield return PathToken.ForName(name);
                continue;
            }

            if (bracketIndex > 0)
            {
                yield return PathToken.ForName(name.Substring(0, bracketIndex));
            }

            var remainder = name.Substring(bracketIndex);
            while (remainder.Length > 0)
            {
                var closeIndex = remainder.IndexOf(']');
                var indexText = remainder.Substring(1, closeIndex - 1);
                yield return PathToken.ForIndex(int.Parse(indexText));
                remainder = remainder.Substring(closeIndex + 1);
            }
        }
    }

    private readonly struct PathToken
    {
        private PathToken(string? name, int index, bool isIndex)
        {
            Name = name;
            Index = index;
            IsIndex = isIndex;
        }

        public string? Name { get; }

        public int Index { get; }

        public bool IsIndex { get; }

        public static PathToken ForName(string name) => new(name, 0, false);

        public static PathToken ForIndex(int index) => new(null, index, true);
    }
}
