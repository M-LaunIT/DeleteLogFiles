using System.Text.Json;

namespace DeleteLogFiles.Configurator;

internal static class ConfigurationFile
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public static AppConfiguration Load(string path)
    {
        if (!File.Exists(path))
        {
            return new AppConfiguration();
        }

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<AppConfiguration>(json, JsonOptions) ?? new AppConfiguration();
    }

    public static void Save(string path, AppConfiguration configuration)
    {
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var json = JsonSerializer.Serialize(configuration, JsonOptions);
        File.WriteAllText(path, json);
    }
}
