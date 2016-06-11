using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GraphX.Controls;
using Magisterka.Domain.ViewModels;
using Magisterka.VisualEcosystem.ErrorHandling;
using Magisterka.VisualEcosystem.Extensions;

namespace Magisterka.VisualEcosystem.Validators
{
    public class ConfigurationValidator
    {
        private ErrorDisplayer _errorDisplayer;

        public ConfigurationValidator(ErrorDisplayer errorDisplayer)
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
