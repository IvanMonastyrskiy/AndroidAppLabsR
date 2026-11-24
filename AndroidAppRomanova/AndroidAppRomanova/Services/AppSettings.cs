using System;
using System.IO;

namespace AndroidAppRomanova.Services;

public sealed class AppSettings
{
    private static readonly string FilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "android_app_labs_settings.txt");

    public static AppSettings Current { get; } = Load();

    public string PageTitle { get; set; } = "Вычисление y = a + b + c";
    public string Background { get; set; } = "#FFF7F7F7";
    public string Foreground { get; set; } = "#FF202020";
    public double FontSize { get; set; } = 16;

    public static readonly string[] BackgroundOptions =
        { "Белый", "Светло-серый", "Голубой", "Светло-зелёный", "Тёмный" };
    public static readonly string[] ForegroundOptions =
        { "Чёрный", "Белый", "Синий", "Зелёный" };

    public static string BackgroundFromName(string name) => name switch
    {
        "Светло-серый" => "#FFECEFF1",
        "Голубой" => "#FFE3F2FD",
        "Светло-зелёный" => "#FFE8F5E9",
        "Тёмный" => "#FF30343B",
        _ => "#FFF7F7F7"
    };

    public static string ForegroundFromName(string name) => name switch
    {
        "Белый" => "#FFFFFFFF",
        "Синий" => "#FF1565C0",
        "Зелёный" => "#FF2E7D32",
        _ => "#FF202020"
    };

    public static string BackgroundName(string value) =>
        Array.Find(BackgroundOptions, x => BackgroundFromName(x) == value) ?? "Белый";

    public static string ForegroundName(string value) =>
        Array.Find(ForegroundOptions, x => ForegroundFromName(x) == value) ?? "Чёрный";

    public void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
            File.WriteAllLines(FilePath, new[]
            {
                PageTitle, Background, Foreground, FontSize.ToString(System.Globalization.CultureInfo.InvariantCulture)
            });
        }
        catch { }
    }

    private static AppSettings Load()
    {
        var result = new AppSettings();
        try
        {
            if (!File.Exists(FilePath)) return result;
            var lines = File.ReadAllLines(FilePath);
            if (lines.Length > 0 && !string.IsNullOrWhiteSpace(lines[0])) result.PageTitle = lines[0];
            if (lines.Length > 1 && !string.IsNullOrWhiteSpace(lines[1])) result.Background = lines[1];
            if (lines.Length > 2 && !string.IsNullOrWhiteSpace(lines[2])) result.Foreground = lines[2];
            if (lines.Length > 3 && double.TryParse(lines[3], System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out var size))
                result.FontSize = Math.Clamp(size, 12, 28);
        }
        catch { }
        return result;
    }
}