using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace AndroidAppRomanova.ViewModels;

public partial class GameViewModel : NotifyViewModelBase
{
    public ObservableCollection<TileViewModel> Tiles { get; } = new();
    [ObservableProperty] private int boardSize = 4;
    [ObservableProperty] private int moves;
    [ObservableProperty] private string customLayout = "1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 0";
    [ObservableProperty] private int columns = 4;
    private int _emptyIndex = 15;
    private int _lastMoved = -1;
    private readonly Random _random = new();

    public GameViewModel() => CreateRandom();

    partial void OnBoardSizeChanged(int value)
    {
        if (value is < 3 or > 5) BoardSize = Math.Clamp(value, 3, 5);
        Columns = BoardSize;
    }

    [RelayCommand]
    private void CreateRandom()
    {
        var values = Enumerable.Range(0, BoardSize * BoardSize).ToArray();
        int empty = values.Length - 1;
        // Перемешиваем только допустимыми ходами — поле всегда остаётся решаемым.
        for (int i = 0; i < 250; i++)
        {
            var options = Neighbours(empty, BoardSize).ToArray();
            int next = options[_random.Next(options.Length)];
            (values[empty], values[next]) = (values[next], values[empty]);
            empty = next;
        }
        SetBoard(values);
        Moves = 0;
        _lastMoved = -1;
        CustomLayout = string.Join(" ", values);
    }

    [RelayCommand]
    private void ApplyLayout()
    {
        var parts = (CustomLayout ?? "").Split(new[] { ' ', ',', ';', '\n', '\r', '\t' },
            StringSplitOptions.RemoveEmptyEntries);
        int count = BoardSize * BoardSize;
        if (parts.Length != count || !parts.All(x => int.TryParse(x, out _)))
        {
            ShowNotification("Ошибка расстановки", $"Введите {count} целых чисел от 0 до {count - 1}.");
            return;
        }
        var values = parts.Select(int.Parse).ToArray();
        if (!values.OrderBy(x => x).SequenceEqual(Enumerable.Range(0, count)))
        {
            ShowNotification("Ошибка расстановки", "Каждое число от 0 до N−1 должно встретиться ровно один раз. 0 — пустая клетка.");
            return;
        }
        if (!IsSolvable(values, BoardSize))
        {
            ShowNotification("Нерешаемая конфигурация", "Для этой расстановки нельзя получить правильный порядок ходами.");
            return;
        }
        SetBoard(values);
        Moves = 0;
        _lastMoved = -1;
    }

    [RelayCommand]
    private void Clear()
    {
        BoardSize = 4;
        CreateRandom();
    }

    private void SetBoard(int[] values)
    {
        Tiles.Clear();
        for (int i = 0; i < values.Length; i++)
        {
            var tile = new TileViewModel(i, MoveTile);
            tile.Text = values[i] == 0 ? "" : values[i].ToString();
            Tiles.Add(tile);
            if (values[i] == 0) _emptyIndex = i;
        }
        RefreshColors();
    }

    private void MoveTile(int index)
    {
        if (!Neighbours(_emptyIndex, BoardSize).Contains(index)) return;
        var empty = Tiles[_emptyIndex];
        var selected = Tiles[index];
        (empty.Text, selected.Text) = (selected.Text, empty.Text);
        _emptyIndex = index;
        _lastMoved = index;
        Moves++;
        RefreshColors();
        if (Tiles.Take(Tiles.Count - 1).Select(x => x.Text).SequenceEqual(
            Enumerable.Range(1, Tiles.Count - 1).Select(x => x.ToString())))
            ShowNotification("Победа!", $"Головоломка решена за {Moves} ходов.");
    }

    private void RefreshColors()
    {
        for (int i = 0; i < Tiles.Count; i++)
        {
            Tiles[i].Background = i == _lastMoved ? new SolidColorBrush(Color.Parse("#FFFFCC80"))
                : i != _emptyIndex && Neighbours(_emptyIndex, BoardSize).Contains(i)
                    ? new SolidColorBrush(Color.Parse("#FFC8E6C9"))
                    : new SolidColorBrush(Color.Parse("#FFE0E0E0"));
            Tiles[i].Foreground = Brushes.Black;
        }
    }

    private static System.Collections.Generic.IEnumerable<int> Neighbours(int index, int size)
    {
        int row = index / size, col = index % size;
        if (row > 0) yield return index - size;
        if (row < size - 1) yield return index + size;
        if (col > 0) yield return index - 1;
        if (col < size - 1) yield return index + 1;
    }

    private static bool IsSolvable(int[] values, int size)
    {
        int inversions = 0;
        for (int i = 0; i < values.Length; i++)
            for (int j = i + 1; j < values.Length; j++)
                if (values[i] != 0 && values[j] != 0 && values[i] > values[j]) inversions++;
        if (size % 2 == 1) return inversions % 2 == 0;
        int rowFromBottom = size - (_emptyIndexStatic(values, size) / size);
        return (inversions + rowFromBottom) % 2 == 1;
    }

    private static int _emptyIndexStatic(int[] values, int size) => Array.IndexOf(values, 0);
}