using System;
using System.IO;

namespace StickyNoteApp;

public static class Constants
{
    public const string AppName = "StickyNoteApp";
    public const string SettingsFileName = "settings.json";

    public const double DefaultWidth = 228;
    public const double DefaultHeight = 173;
    public const double MinWidth = 150;
    public const double MinHeight = 100;

    public const double LineHeight = 1.0;
    public const double CharacterSpacing = 0;
    public const double ListIndent = 14;

    public const string DefaultFontFamily = "Yu Gothic UI";
    public const double DefaultFontSize = 14;
    public const string DefaultFontColor = "#333333";

    public static string AppDataPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        AppName);
}
