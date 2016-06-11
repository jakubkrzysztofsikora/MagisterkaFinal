using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Magisterka.VisualEcosystem.Validators
{
    public class ValidationErrorDisplayer
    {
        public void DisplayError(eValidationErrorTypes errorType, string message)
        {
            string title = GetErrorTitle(errorType);
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private string GetErrorTitle(eValidationErrorTypes errorType)
        {
            switch (errorType)
            {
                case eValidationErrorTypes.PathConfiguration:
                    return Application.Current.Resources["PathConfError"].ToString();
                case eValidationErrorTypes.GraphConfiguration:
                    return Application.Current.Resources["GraphConfError"].ToString();
                case eValidationErrorTypes.General:
                    return Application.Current.Resources["GeneralError"].ToString();
                default:
                    return Application.Current.Resources["UnknownPathfindingError"].ToString();
            }
        }
    }
}
