using AndroidAppRomanova.ViewModels;
using Avalonia.Controls;

namespace AndroidAppRomanova.Views;

public partial class MatrixPage : UserControl
{
    public MatrixPage()
    {
        InitializeComponent();
        DataContext = new MatrixViewModel();
        SizeChanged += (_, _) => ApplyOrientation();
        ApplyOrientation();
    }

    private void ApplyOrientation()
    {
        if (MainLayout is null) return;
        // При альбомной ориентации блоки управления и матрица располагаются рядом.
        MainLayout.Orientation = Bounds.Width > Bounds.Height
            ? Avalonia.Layout.Orientation.Horizontal
            : Avalonia.Layout.Orientation.Vertical;
    }
}