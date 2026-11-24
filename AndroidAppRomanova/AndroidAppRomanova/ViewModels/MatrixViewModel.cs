using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Globalization;

namespace AndroidAppRomanova.ViewModels;

public partial class MatrixViewModel : NotifyViewModelBase
{
    public ObservableCollection<CellViewModel> Cells { get; } = new();
    [ObservableProperty] private string rowsText = "4";
    [ObservableProperty] private string columnsText = "4";
    [ObservableProperty] private int rows = 4;
    [ObservableProperty] private int columns = 4;
    [ObservableProperty] private string resultText = "Результат пока не рассчитан.";
    private readonly Random _random = new();

    public MatrixViewModel() => CreateMatrix();

    [RelayCommand]
    private void CreateMatrix()
    {
        if (!int.TryParse((RowsText ?? "").Trim(), out int r) ||
            !int.TryParse((ColumnsText ?? "").Trim(), out int c) ||
            r < 2 || r > 10 || c < 1 || c > 10)
        {
            ShowNotification("Неверный размер", "Количество строк: 2–10, столбцов: 1–10.");
            return;
        }
        Rows = r;
        Columns = c;
        Cells.Clear();
        for (int i = 0; i < r; i++)
            for (int j = 0; j < c; j++)
                Cells.Add(new CellViewModel(i, j));
        ResultText = "Матрица создана. Заполните её и нажмите «Выполнить вариант 20».";
    }

    [RelayCommand]
    private void FillRandom()
    {
        foreach (var cell in Cells) cell.Value = _random.Next(-99, 100).ToString(CultureInfo.InvariantCulture);
        ResultText = "Матрица заполнена случайными числами.";
    }

    [RelayCommand]
    private void SwapRows()
    {
        if (Rows < 4 || Rows % 2 != 0)
        {
            ShowNotification("Невозможно выполнить", "Для двух средних строк требуется чётное количество строк не менее 4.");
            return;
        }

        int[,] data = new int[Rows, Columns];
        foreach (var cell in Cells)
        {
            if (!int.TryParse((cell.Value ?? "").Trim(), out int value))
            {
                ShowNotification("Ошибка ввода", $"Проверьте ячейку ({cell.Row + 1}, {cell.Column + 1}). Введите целое число.");
                return;
            }
            data[cell.Row, cell.Column] = value;
        }

        int firstMiddle = Rows / 2 - 1;
        int secondMiddle = Rows / 2;
        for (int col = 0; col < Columns; col++)
        {
            (data[0, col], data[firstMiddle, col]) = (data[firstMiddle, col], data[0, col]);
            (data[Rows - 1, col], data[secondMiddle, col]) = (data[secondMiddle, col], data[Rows - 1, col]);
        }

        foreach (var cell in Cells)
            cell.Value = data[cell.Row, cell.Column].ToString(CultureInfo.InvariantCulture);

        ResultText = $"Вариант 20 выполнен: первая строка обменена с {firstMiddle + 1}-й, последняя — с {secondMiddle + 1}-й (двумя средними строками).";
    }

    [RelayCommand]
    private void Clear()
    {
        foreach (var cell in Cells) cell.Value = "0";
        ResultText = "Все элементы обнулены.";
    }
}