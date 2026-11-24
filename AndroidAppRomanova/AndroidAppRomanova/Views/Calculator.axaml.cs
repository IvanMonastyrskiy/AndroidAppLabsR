using AndroidAppRomanova.ViewModels;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace AndroidAppRomanova.Views
{
    public partial class Calculator : UserControl
    {
        public Calculator()
        {
            InitializeComponent();
            DataContext = new CalculatorViewModel();
        }
    }

}
