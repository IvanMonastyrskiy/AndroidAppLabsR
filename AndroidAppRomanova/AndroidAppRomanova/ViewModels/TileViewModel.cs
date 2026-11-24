using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace AndroidAppRomanova.ViewModels;

public partial class TileViewModel : ObservableObject
{
    private readonly Action<int> _onClick;
    public int Index { get; }
    [ObservableProperty] private string text = "";
    [ObservableProperty] private IBrush background = Brushes.LightGray;
    [ObservableProperty] private IBrush foreground = Brushes.Black;

    public TileViewModel(int index, Action<int> onClick)
    {
        Index = index;
        _onClick = onClick;
    }

    [RelayCommand]
    private void Press() => _onClick(Index);
}