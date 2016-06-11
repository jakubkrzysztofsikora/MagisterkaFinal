using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GraphX.Controls;
using Magisterka.Domain.ViewModels;
using Magisterka.VisualEcosystem.Extensions;

namespace Magisterka.VisualEcosystem.Validators
{
    public class ConfigurationValidator
    {
        private ValidationErrorDisplayer _errorDisplayer;

        public ConfigurationValidator(ValidationErrorDisplayer errorDisplayer)
        {
            _errorDisplayer = errorDisplayer;
        }

        public bool ValidateCanBeDefinedPosition(VertexControl vertex)
        {
            NodeView node = vertex.GetNodeView();

            if (node.LogicNode.IsBlocked || node.LogicNode.IsStartingNode || node.LogicNode.IsTargetNode)
            {
                _errorDisplayer.DisplayError(eValidationErrorTypes.PathConfiguration,
                    Application.Current.Resources["CannotBeDefinedPosition"].ToString());
                return false;
            }

            return true;
        }
    }
}
