using System;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;

namespace Magisterka.VisualEcosystem.InputModals
{
    /// <summary>
    /// Interaction logic for ChangeEdgeCostModal.xaml
    /// </summary>
    public partial class ChangeEdgeCostModal : MetroWindow
    {
        public ChangeEdgeCostModal(string question, int defaultAnswer)
        {
            InitializeComponent();
            Label.Content = question;
            NewEdgeCost.Text = defaultAnswer.ToString();
        }

        public int Answer => Convert.ToInt32(NewEdgeCost.Text);

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            NewEdgeCost.SelectAll();
            NewEdgeCost.Focus();
        }

        private new void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            try
            {
               var newValue = Convert.ToInt32($"{NewEdgeCost.Text}{e.Text}");
            }
            catch (Exception)
            {
                e.Handled = true;
            }
        }
    }
}
