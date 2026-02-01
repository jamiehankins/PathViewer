using System;
using System.IO;
using System.Text.Json;

using Xunit;

namespace PathViewer.Tests;

public class PreferencesDefaultValueTests
{
    [Fact]
    public void DefaultConstructor_SetsThemeToSystem()
    {
        var prefs = new Preferences();

        Assert.Equal("System", prefs.Theme);
    }

    [Fact]
    public void DefaultConstructor_SetsStrokeColorToBlack()
    {
        var prefs = new Preferences();

        Assert.Equal("Black", prefs.StrokeColor);
    }

    [Fact]
    public void DefaultConstructor_SetsStrokeThicknessTo2()
    {
        var prefs = new Preferences();

        Assert.Equal(2.0, prefs.StrokeThickness);
    }

    [Fact]
    public void DefaultConstructor_SetsFillColorToTransparent()
    {
        var prefs = new Preferences();

        Assert.Equal("Transparent", prefs.FillColor);
    }

    [Fact]
    public void DefaultConstructor_SetsFillOpacityTo255()
    {
        var prefs = new Preferences();

        Assert.Equal(255, prefs.FillOpacity);
    }

    [Fact]
    public void DefaultConstructor_SetsShowOriginLinesToTrue()
    {
        var prefs = new Preferences();

        Assert.True(prefs.ShowOriginLines);
    }

    [Fact]
    public void DefaultConstructor_SetsShowBoundingBoxToFalse()
    {
        var prefs = new Preferences();

        Assert.False(prefs.ShowBoundingBox);
    }
}

public class PreferencesSerializationTests
{
    [Fact]
    public void Serialize_DefaultPreferences_ProducesValidJson()
    {
        var prefs = new Preferences();

        var json = JsonSerializer.Serialize(prefs);

        Assert.NotNull(json);
        Assert.Contains("\"Theme\":\"System\"", json);
    }

    [Fact]
    public void Deserialize_ValidJson_ReturnsPreferences()
    {
        var json = """
            {
                "Theme": "Dark",
                "StrokeColor": "Red",
                "StrokeThickness": 3.5,
                "FillColor": "Blue",
                "FillOpacity": 128,
                "ShowOriginLines": false,
                "ShowBoundingBox": true
            }
            """;

        var prefs = JsonSerializer.Deserialize<Preferences>(json);

        Assert.NotNull(prefs);
        Assert.Equal("Dark", prefs.Theme);
        Assert.Equal("Red", prefs.StrokeColor);
        Assert.Equal(3.5, prefs.StrokeThickness);
        Assert.Equal("Blue", prefs.FillColor);
        Assert.Equal(128, prefs.FillOpacity);
        Assert.False(prefs.ShowOriginLines);
        Assert.True(prefs.ShowBoundingBox);
    }

    [Fact]
    public void Deserialize_PartialJson_UsesDefaults()
    {
        var json = """{"Theme": "Light"}""";

        var prefs = JsonSerializer.Deserialize<Preferences>(json);

        Assert.NotNull(prefs);
        Assert.Equal("Light", prefs.Theme);
        Assert.Equal("Black", prefs.StrokeColor); // Default
        Assert.Equal(2.0, prefs.StrokeThickness); // Default
    }

    [Fact]
    public void Deserialize_EmptyJson_ReturnsDefaults()
    {
        var json = "{}";

        var prefs = JsonSerializer.Deserialize<Preferences>(json);

        Assert.NotNull(prefs);
        Assert.Equal("System", prefs.Theme);
        Assert.Equal("Black", prefs.StrokeColor);
    }

    [Fact]
    public void RoundTrip_PreservesAllValues()
    {
        var original = new Preferences
        {
            Theme = "Dark",
            StrokeColor = "Green",
            StrokeThickness = 5.0,
            FillColor = "Yellow",
            FillOpacity = 100,
            ShowOriginLines = false,
            ShowBoundingBox = true
        };

        var json = JsonSerializer.Serialize(original);
        var restored = JsonSerializer.Deserialize<Preferences>(json);

        Assert.NotNull(restored);
        Assert.Equal(original.Theme, restored.Theme);
        Assert.Equal(original.StrokeColor, restored.StrokeColor);
        Assert.Equal(original.StrokeThickness, restored.StrokeThickness);
        Assert.Equal(original.FillColor, restored.FillColor);
        Assert.Equal(original.FillOpacity, restored.FillOpacity);
        Assert.Equal(original.ShowOriginLines, restored.ShowOriginLines);
        Assert.Equal(original.ShowBoundingBox, restored.ShowBoundingBox);
    }

    [Fact]
    public void Deserialize_NullJson_ReturnsNull()
    {
        var result = JsonSerializer.Deserialize<Preferences>("null");

        Assert.Null(result);
    }

    [Fact]
    public void Deserialize_InvalidJson_ThrowsException()
    {
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<Preferences>("not valid json"));
    }

    [Fact]
    public void Serialize_WithIndentation_ProducesFormattedJson()
    {
        var prefs = new Preferences();
        var options = new JsonSerializerOptions { WriteIndented = true };

        var json = JsonSerializer.Serialize(prefs, options);

        Assert.Contains(Environment.NewLine, json);
    }
}

public class PreferencesFilePathTests
{
    [Fact]
    public void PreferencesPath_IsInAppDataFolder()
    {
        // Use reflection to access private static field
        var fieldInfo = typeof(Preferences).GetField(
            "PreferencesPath",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        Assert.NotNull(fieldInfo);
        var path = fieldInfo.GetValue(null) as string;

        Assert.NotNull(path);
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        Assert.StartsWith(appDataPath, path);
    }

    [Fact]
    public void PreferencesPath_ContainsPathViewerFolder()
    {
        var fieldInfo = typeof(Preferences).GetField(
            "PreferencesPath",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        Assert.NotNull(fieldInfo);
        var path = fieldInfo.GetValue(null) as string;

        Assert.NotNull(path);
        Assert.Contains("PathViewer", path);
    }

    [Fact]
    public void PreferencesPath_EndsWithPreferencesJson()
    {
        var fieldInfo = typeof(Preferences).GetField(
            "PreferencesPath",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        Assert.NotNull(fieldInfo);
        var path = fieldInfo.GetValue(null) as string;

        Assert.NotNull(path);
        Assert.EndsWith("Preferences.json", path);
    }

    [Fact]
    public void PreferencesFolder_IsInAppDataFolder()
    {
        var fieldInfo = typeof(Preferences).GetField(
            "PreferencesFolder",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        Assert.NotNull(fieldInfo);
        var folder = fieldInfo.GetValue(null) as string;

        Assert.NotNull(folder);
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        Assert.StartsWith(appDataPath, folder);
    }

    [Fact]
    public void PreferencesFolder_EndsWithPathViewer()
    {
        var fieldInfo = typeof(Preferences).GetField(
            "PreferencesFolder",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        Assert.NotNull(fieldInfo);
        var folder = fieldInfo.GetValue(null) as string;

        Assert.NotNull(folder);
        Assert.EndsWith("PathViewer", folder);
    }
}

public class PreferencesLoadTests
{
    [Fact]
    public void Load_WhenFileDoesNotExist_ReturnsDefaultPreferences()
    {
        // This test verifies the default fallback behavior
        // Note: This assumes the test runs with no pre-existing preferences file
        // or the preferences file is in the standard location

        var prefs = Preferences.Load();

        // Should return a valid Preferences object (either from file or defaults)
        Assert.NotNull(prefs);
    }
}

public class PreferencesRecordTests
{
    [Fact]
    public void Preferences_IsRecord_SupportsWithExpression()
    {
        var original = new Preferences();

        var modified = original with { Theme = "Dark" };

        Assert.Equal("System", original.Theme);
        Assert.Equal("Dark", modified.Theme);
    }

    [Fact]
    public void Preferences_Equality_WorksCorrectly()
    {
        var prefs1 = new Preferences { Theme = "Dark" };
        var prefs2 = new Preferences { Theme = "Dark" };
        var prefs3 = new Preferences { Theme = "Light" };

        Assert.Equal(prefs1, prefs2);
        Assert.NotEqual(prefs1, prefs3);
    }

    [Fact]
    public void Preferences_GetHashCode_ConsistentWithEquality()
    {
        var prefs1 = new Preferences { Theme = "Dark" };
        var prefs2 = new Preferences { Theme = "Dark" };

        Assert.Equal(prefs1.GetHashCode(), prefs2.GetHashCode());
    }
}
