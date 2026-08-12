using System.Collections;
using System.Text.Json;

namespace Loscardos.TikTokPartnerSdk.Core.Http;

internal static class TikTokRequestBodySerializer
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public static string Serialize(object body)
        => JsonSerializer.Serialize(RemoveNullDictionaryValues(body), SerializerOptions);

    private static object? RemoveNullDictionaryValues(object? value)
    {
        if (value is null || value is string)
        {
            return value;
        }

        if (value is IEnumerable<KeyValuePair<string, object?>> dictionary)
        {
            return dictionary
                .Where(static item => item.Value is not null)
                .ToDictionary(
                    static item => item.Key,
                    static item => RemoveNullDictionaryValues(item.Value),
                    StringComparer.Ordinal);
        }

        if (value is IEnumerable sequence)
        {
            return sequence
                .Cast<object?>()
                .Select(RemoveNullDictionaryValues)
                .ToArray();
        }

        return value;
    }
}
