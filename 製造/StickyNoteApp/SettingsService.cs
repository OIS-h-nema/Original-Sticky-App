using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace StickyNoteApp;

public static class SettingsService
{
    private static readonly string SettingsPath =
        Path.Combine(Constants.AppDataPath, Constants.SettingsFileName);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static AppSettings Load()
    {
        try
        {
            if (!Directory.Exists(Constants.AppDataPath))
            {
                Directory.CreateDirectory(Constants.AppDataPath);
            }

            if (!File.Exists(SettingsPath))
            {
                return new AppSettings();
            }

            var json = File.ReadAllText(SettingsPath);
            var settings = JsonSerializer.Deserialize<AppSettings>(json, JsonOptions) ?? new AppSettings();
            settings.ValidateAndFix();
            return settings;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Settings load failed: {ex}");
            return new AppSettings();
        }
    }

    public static async Task SaveAsync(AppSettings settings)
    {
        try
        {
            if (!Directory.Exists(Constants.AppDataPath))
            {
                Directory.CreateDirectory(Constants.AppDataPath);
            }

            var json = JsonSerializer.Serialize(settings, JsonOptions);
            await File.WriteAllTextAsync(SettingsPath, json).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Settings save failed: {ex}");
        }
    }

    public static void Save(AppSettings settings)
    {
        try
        {
            if (!Directory.Exists(Constants.AppDataPath))
            {
                Directory.CreateDirectory(Constants.AppDataPath);
            }

            var json = JsonSerializer.Serialize(settings, JsonOptions);
            File.WriteAllText(SettingsPath, json);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Settings save failed: {ex}");
        }
    }
}
