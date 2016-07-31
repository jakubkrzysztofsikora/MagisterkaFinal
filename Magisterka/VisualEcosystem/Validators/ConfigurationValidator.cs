using System.Windows;
using GraphX.Controls;
using Magisterka.Domain.ExceptionContracts;
using Magisterka.Domain.ViewModels;
using Magisterka.VisualEcosystem.ErrorHandling;
using Magisterka.VisualEcosystem.Extensions;

namespace Magisterka.VisualEcosystem.Validators
{
    public class ConfigurationValidator : IConfigurationValidator
    {
        private readonly IErrorDisplayer _errorDisplayer;

        public ConfigurationValidator(IErrorDisplayer errorDisplayer)
        {
            _errorDisplayer = errorDisplayer;
        }

        public bool ValidateCanBeDefinedPosition(VertexControl vertex)
        {
            NodeView node = vertex.GetNodeView();

            if (node.LogicNode.IsBlocked || node.LogicNode.IsStartingNode || node.LogicNode.IsTargetNode)
            {
                _errorDisplayer.DisplayError(eErrorTypes.PathConfiguration,
                    Application.Current.Resources["CannotBeDefinedPosition"].ToString());
                return false;
            }

            return true;
        }
    }
}
