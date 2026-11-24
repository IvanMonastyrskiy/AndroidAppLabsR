using AndroidAppRomanova.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Globalization;

namespace AndroidAppRomanova.ViewModels;

public partial class CalculatorViewModel : NotifyViewModelBase
{
    private const double C = 25;

    [ObservableProperty] private string? wText = "";
    [ObservableProperty] private string? vText = "";
    [ObservableProperty] private string aResult = "—";
    [ObservableProperty] private string bResult = "—";
    [ObservableProperty] private string yResult = "—";

    [ObservableProperty] private string pageTitle = AppSettings.Current.PageTitle;
    [ObservableProperty] private string pageBackground = AppSettings.Current.Background;
    [ObservableProperty] private string pageForeground = AppSettings.Current.Foreground;
    [ObservableProperty] private double pageFontSize = AppSettings.Current.FontSize;

    [ObservableProperty] private string draftTitle = AppSettings.Current.PageTitle;
    [ObservableProperty] private string selectedBackground = AppSettings.BackgroundName(AppSettings.Current.Background);
    [ObservableProperty] private string selectedForeground = AppSettings.ForegroundName(AppSettings.Current.Foreground);
    [ObservableProperty] private string draftFontSize = AppSettings.Current.FontSize.ToString(CultureInfo.InvariantCulture);
    [ObservableProperty] private bool isSettingsOpen;

    public string[] BackgroundOptions => AppSettings.BackgroundOptions;
    public string[] ForegroundOptions => AppSettings.ForegroundOptions;

    [RelayCommand]
    private void Calculate()
    {
        if (!TryParse(WText, out var w) || !TryParse(VText, out var v))
        {
            ShowNotification("Ошибка ввода", "Введите числа w и v, например 45 и 2.5");
            return;
        }
        if (v == 0)
        {
            ShowNotification("Деление на ноль", "v = 0, а b = w / v. Введите v, не равное нулю.");
            return;
        }

        double b = w / v;
        double denominator = 1 - (v + b / 13);
        if (Math.Abs(denominator) < 1e-12)
        {
            ShowNotification("Деление на ноль", "Знаменатель равен нулю. Измените w или v.");
            return;
        }

        double a = Math.Cbrt(w - 20) / denominator;
        double y = a + b + C;
        if (double.IsNaN(y) || double.IsInfinity(y))
        {
            ShowNotification("Ошибка вычисления", "Результат не определён.");
            return;
        }
        AResult = Format(a);
        BResult = Format(b);
        YResult = Format(y);
    }

    [RelayCommand]
    private void Clear()
    {
        WText = "";
        VText = "";
        AResult = BResult = YResult = "—";
    }

    [RelayCommand]
    private void OpenSettings()
    {
        DraftTitle = PageTitle;
        SelectedBackground = AppSettings.BackgroundName(PageBackground);
        SelectedForeground = AppSettings.ForegroundName(PageForeground);
        DraftFontSize = PageFontSize.ToString(CultureInfo.InvariantCulture);
        IsSettingsOpen = true;
    }

    [RelayCommand]
    private void CloseSettings() => IsSettingsOpen = false;

    [RelayCommand]
    private void ApplySettings()
    {
        if (!double.TryParse((DraftFontSize ?? "").Trim().Replace(',', '.'),
            NumberStyles.Float, CultureInfo.InvariantCulture, out var size) || size < 12 || size > 28)
        {
            ShowNotification("Некорректный размер", "Размер шрифта должен быть от 12 до 28.");
            return;
        }

        PageTitle = string.IsNullOrWhiteSpace(DraftTitle) ? "Вычисление y = a + b + c" : DraftTitle.Trim();
        PageBackground = AppSettings.BackgroundFromName(SelectedBackground ?? "Белый");
        PageForeground = AppSettings.ForegroundFromName(SelectedForeground ?? "Чёрный");
        PageFontSize = size;

        var settings = AppSettings.Current;
        settings.PageTitle = PageTitle;
        settings.Background = PageBackground;
        settings.Foreground = PageForeground;
        settings.FontSize = PageFontSize;
        settings.Save();
        IsSettingsOpen = false;
        ShowNotification("Настройки применены", "Выбранное оформление сохранено.");
    }

    private static bool TryParse(string? s, out double value) =>
        double.TryParse((s ?? "").Trim().Replace(',', '.'), NumberStyles.Float,
            CultureInfo.InvariantCulture, out value);

    private static string Format(double x) => x.ToString("0.####", CultureInfo.InvariantCulture);
}