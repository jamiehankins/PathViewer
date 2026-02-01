using PathViewer.Services;

namespace PathViewer.Tests.Mocks;

public class MockPreferencesService : IPreferencesService
{
    public Preferences PreferencesToReturn { get; set; } = new();
    public Preferences? LastSavedPreferences { get; private set; }
    public int LoadCallCount { get; private set; }
    public int SaveCallCount { get; set; }

    public Preferences Load()
    {
        LoadCallCount++;
        return PreferencesToReturn;
    }

    public void Save(Preferences preferences)
    {
        SaveCallCount++;
        LastSavedPreferences = preferences;
    }
}
