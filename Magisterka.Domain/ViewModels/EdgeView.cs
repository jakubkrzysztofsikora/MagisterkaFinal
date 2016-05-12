using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphX.PCL.Common.Models;
using QuickGraph;

namespace Magisterka.Domain.ViewModels
{
    public class EdgeView : EdgeBase<NodeView>
    {
        public EdgeView(NodeView source, NodeView target, double weight = 1) : base(source, target, weight)
        {
        }
    }
}
