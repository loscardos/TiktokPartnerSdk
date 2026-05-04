namespace TikTokPartnerSdk.SampleConsole;

public static class SandboxEnvFile
{
    public static string DefaultPath => Path.GetFullPath(Path.Combine(
        AppContext.BaseDirectory,
        "..",
        "..",
        "..",
        "..",
        "..",
        ".token",
        "tiktok-sandbox.env"));

    public static void UpsertValues(string path, IReadOnlyDictionary<string, string> values)
    {
        var lines = File.Exists(path)
            ? File.ReadAllLines(path).ToList()
            : new List<string>();
        var remaining = new Dictionary<string, string>(values, StringComparer.Ordinal);

        for (var index = 0; index < lines.Count; index++)
        {
            var line = lines[index];
            var separator = line.IndexOf('=', StringComparison.Ordinal);
            if (separator <= 0)
            {
                continue;
            }

            var key = line[..separator].Trim();
            if (remaining.Remove(key, out var value))
            {
                lines[index] = $"{key}={value}";
            }
        }

        foreach (var (key, value) in remaining)
        {
            lines.Add($"{key}={value}");
        }

        Directory.CreateDirectory(Path.GetDirectoryName(path) ?? ".");
        File.WriteAllText(path, string.Join(Environment.NewLine, lines) + Environment.NewLine);
    }
}
