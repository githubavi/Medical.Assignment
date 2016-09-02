using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using Microsoft.Windows.Controls.Ribbon;

namespace WPF.Assignment
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IView
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new DailyMedViewModel(this);
            SetCursor();
        }

        public void SetCursor()
        {
            this.txtSearch.Focus();
        }


        public void SetSuggestionBox()
        {
            this.listSuggestion.Focus();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            RadialGradientBrush radialGradientBrush = new RadialGradientBrush(Colors.Orange, Colors.Pink);
            this.Resources["r1"] = radialGradientBrush;
        }
    }
}
