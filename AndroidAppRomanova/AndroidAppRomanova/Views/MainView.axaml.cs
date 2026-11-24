
using Avalonia;
using Avalonia.Controls;

namespace AndroidAppRomanova.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }
    private void Open_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Content = new Calculator
        {
            DataContext = new ViewModels.CalculatorViewModel()
        };
    }
}