using System;
using System.Collections.Generic;
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

namespace MIDIparser.Views.VisualEventParamViews
{
    /// <summary>
    /// Interaction logic for ChangeRotationLinearParamView.xaml
    /// </summary>
    public partial class ChangeRotationLinearParamView : UserControl
    {
        public ChangeRotationLinearParamView()
        {
            InitializeComponent();
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void DecimalValidationTextBox(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            bool approvedDecimalPoint = false;
            bool approvedMinusOnFront = false;

            if (e.Text == ".")
            {
                if (!((TextBox)sender).Text.Contains("."))
                    approvedDecimalPoint = true;
            }
            if (e.Text == "-")
            {
                if (((TextBox)sender).Text.Length == 0)
                {
                    approvedMinusOnFront = true;
                }
            }

            if (!(char.IsDigit(e.Text, e.Text.Length - 1) || approvedDecimalPoint || approvedMinusOnFront))
                e.Handled = true;
        }
    }
}
