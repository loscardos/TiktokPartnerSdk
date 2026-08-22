using System.Collections;

namespace Loscardos.TikTokPartnerSdk.Core.Managers;

internal static class TikTokGeneratedRequestMap
{
    public static void AddOptional(
        IDictionary<string, object?> target,
        string name,
        object? value)
    {
        if (IsUnspecified(value))
        {
            return;
        }

        target[name] = value;
    }

    private static bool IsUnspecified(object? value)
    {
        if (value is null || value is string { Length: 0 })
        {
            return true;
        }

        if (value is IEnumerable sequence and not string)
        {
            var enumerator = sequence.GetEnumerator();
            try
            {
                return !enumerator.MoveNext();
            }
            finally
            {
                (enumerator as IDisposable)?.Dispose();
            }
        }

        return false;
    }
}
