using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AndroidAppRomanova.ViewModels
{
    public partial class CalculatorViewModel : NotifyViewModelBase
    {
        private const double C = 25;

        [ObservableProperty] private string? wText = "";
        [ObservableProperty] private string? vText = "";

        [ObservableProperty] private string aResult = "—";
        [ObservableProperty] private string bResult = "—";
        [ObservableProperty] private string yResult = "—";

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
                ShowNotification("Деление на ноль", "v = 0, а b = w / v. Введите v не равное нулю.");
                return;
            }

            double b = w / v;
            double denominator = 1 - (v + b / 13);

            if (Math.Abs(denominator) < 1e-12)
            {
                ShowNotification("Деление на ноль",
                    "Знаменатель 1 − (v + b/13) равен нулю. Измените w или v.");
                return;
            }

            double a = Math.Cbrt(w - 20) / denominator;
            double y = a + b + C;

            if (double.IsNaN(y) || double.IsInfinity(y))
            {
                ShowNotification("Ошибка вычисления", "Результат слишком большой или не определён.");
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
            AResult = "—";
            BResult = "—";
            YResult = "—";
        }

        private static bool TryParse(string? s, out double value) =>
            double.TryParse((s ?? "").Trim().Replace(',', '.'),
                            NumberStyles.Float, CultureInfo.InvariantCulture, out value);

        private static string Format(double x) =>
            x.ToString("0.####", CultureInfo.InvariantCulture);
    }
}
