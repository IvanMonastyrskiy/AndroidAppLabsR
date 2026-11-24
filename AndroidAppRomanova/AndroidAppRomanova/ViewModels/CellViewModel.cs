using CommunityToolkit.Mvvm.ComponentModel;

namespace AndroidAppRomanova.ViewModels;

public partial class CellViewModel : ObservableObject
{
    public int Row { get; }
    public int Column { get; }
    [ObservableProperty] private string value = "0";

    public CellViewModel(int row, int column, string value = "0")
    {
        Row = row;
        Column = column;
        Value = value;
    }
}