using AndroidAppRomanova.ViewModels;
using Avalonia.Controls;

namespace AndroidAppRomanova.Views;

public partial class Game : UserControl
{
    public Game()
    {
        InitializeComponent();
        DataContext = new GameViewModel();
    }
}