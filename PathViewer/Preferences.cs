using System;
using System.IO;
using System.Text.Json;

namespace PathViewer;

public record Preferences
{
    public string Theme { get; init; } = "System";
    public string StrokeColor { get; init; } = "Black";
    public double StrokeThickness { get; init; } = 2.0;
    public string FillColor { get; init; } = "Transparent";
    public byte FillOpacity { get; init; } = 255;
    public bool ShowOriginLines { get; init; } = true;
    public bool ShowBoundingBox { get; init; } = false;

    private static readonly string PreferencesFolder = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "PathViewer");
    
    private static readonly string PreferencesPath = Path.Combine(PreferencesFolder, "Preferences.json");

    public static Preferences Load()
    {
        try
        {
            if (File.Exists(PreferencesPath))
            {
                var json = File.ReadAllText(PreferencesPath);
                return JsonSerializer.Deserialize<Preferences>(json) ?? new Preferences();
            }
        }
        catch
        {
            // If loading fails, return default preferences
        }
        return new Preferences();
    }

    public void Save()
    {
        try
        {
            _ = Directory.CreateDirectory(PreferencesFolder);
            var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(PreferencesPath, json);
        }
        catch
        {
            // Silently fail if we can't save preferences
        }
    }
}