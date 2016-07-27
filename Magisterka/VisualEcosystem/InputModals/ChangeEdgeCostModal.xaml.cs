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
using System.Windows.Shapes;

namespace Magisterka.VisualEcosystem.InputModals
{
    /// <summary>
    /// Interaction logic for ChangeEdgeCostModal.xaml
    /// </summary>
    public partial class ChangeEdgeCostModal : Window
    {
        public int Answer => Convert.ToInt32(NewEdgeCost.Text);

        public ChangeEdgeCostModal(string question, int defaultAnswer)
        {
            InitializeComponent();
            Label.Content = question;
            NewEdgeCost.Text = defaultAnswer.ToString();
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            NewEdgeCost.SelectAll();
            NewEdgeCost.Focus();
        }

        private new void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsNumericValue(NewEdgeCost.Text);
        }

        private bool IsNumericValue(string text)
        {
            Regex regex = new Regex(@"\d+");
            return !regex.IsMatch(text);
        }
    }
}
