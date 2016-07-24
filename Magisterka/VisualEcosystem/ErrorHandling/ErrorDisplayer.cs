using System.Windows;

namespace Magisterka.VisualEcosystem.ErrorHandling
{
    public class ErrorDisplayer : IErrorDisplayer
    {
        public void DisplayError(eErrorTypes errorType, string message)
        {
            string title = GetErrorTitle(errorType);
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private string GetErrorTitle(eErrorTypes errorType)
        {
            switch (errorType)
            {
                case eErrorTypes.PathConfiguration:
                    return Application.Current.Resources["PathConfError"].ToString();
                case eErrorTypes.GraphConfiguration:
                    return Application.Current.Resources["GraphConfError"].ToString();
                case eErrorTypes.General:
                    return Application.Current.Resources["GeneralError"].ToString();
                default:
                    return Application.Current.Resources["UnknownPathfindingError"].ToString();
            }
        }
    }
}
