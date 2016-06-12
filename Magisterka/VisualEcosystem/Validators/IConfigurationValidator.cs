using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphX.Controls;

namespace Magisterka.VisualEcosystem.Validators
{
    public interface IConfigurationValidator
    {
        bool ValidateCanBeDefinedPosition(VertexControl vertex);
    }
}
