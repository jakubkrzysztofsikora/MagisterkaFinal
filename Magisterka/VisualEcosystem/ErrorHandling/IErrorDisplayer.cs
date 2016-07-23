using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magisterka.VisualEcosystem.ErrorHandling
{
    public interface IErrorDisplayer
    {
        void DisplayError(eErrorTypes errorType, string message);
    }
}
