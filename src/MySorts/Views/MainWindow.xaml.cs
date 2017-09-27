﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySorts.ViewModels;

namespace MySorts.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            XAxis.LabelFormatter = d => Math.Round(d, 2).ToString(CultureInfo.InvariantCulture);
            YAxis.LabelFormatter = d => Math.Round(d, 2).ToString(CultureInfo.InvariantCulture);
        }

        private void UIElement_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.Text = Regex.Replace(textBox.Text, " {2,}", " ");
                textBox.SelectionStart = textBox.Text.Length;
            }
            Regex regex = new Regex("^([0-9]+ *)+$");
            e.Handled = !regex.IsMatch(e.Text);
        }
    }
}
