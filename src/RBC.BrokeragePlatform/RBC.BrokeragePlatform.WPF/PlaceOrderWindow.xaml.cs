using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RBC.BrokeragePlatform.WPF
{
    /// <summary>
    /// Logique d'interaction pour PlaceOrderWindow.xaml
    /// </summary>
    public partial class PlaceOrderWindow : Window
    {
        private static readonly Regex _numericRegex = new Regex("^[0-9]+$");
        public PlaceOrderWindow()
        {
            InitializeComponent();
        }

        // Allow only digits
        private void NumberBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !_numericRegex.IsMatch(e.Text);
        }

        // Prevent space, minus, etc.
        private void NumberBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void Cancel_ButtonClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
