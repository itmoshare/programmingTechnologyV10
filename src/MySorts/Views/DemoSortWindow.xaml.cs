using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MySorts.ViewModels;

namespace MySorts.Views
{
    /// <summary>
    /// Interaction logic for DemoSortWindow.xaml
    /// </summary>
    public partial class DemoSortWindow : Window
    {
        public DemoSortViewModel ViewModel
        {
            get => DataContext as DemoSortViewModel;
            set => DataContext = value;
        }

        public DemoSortWindow()
        {
            InitializeComponent();
        }
    }
}
