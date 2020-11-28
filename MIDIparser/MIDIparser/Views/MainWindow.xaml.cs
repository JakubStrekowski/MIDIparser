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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using MIDIparser.ViewModels;


namespace MIDIparser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _viewModel;
         

        public MainWindow()
        {
            DataContext = _viewModel;
            _viewModel = new MainWindowViewModel();
            InitializeComponent();
            // The DataContext serves as the starting point of Binding Paths
        }

    }

}
