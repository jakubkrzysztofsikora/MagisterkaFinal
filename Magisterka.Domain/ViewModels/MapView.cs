using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphX.PCL.Logic.Helpers;
using Magisterka.Domain.Graph.MovementSpace;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;
using QuickGraph;

namespace Magisterka.Domain.ViewModels
{
    public class MapView : BidirectionalGraph<NodeView, EdgeView>
    {
        public NodeView GetVertexByLogicNode(Node logicNode)
        {
            return Vertices.SingleOrDefault(nodeView => nodeView.LogicNode == logicNode);
        }

        public long GetNodeIdByPositionGuid(Position position)
        {
            return
                Vertices.Where(nodeView => nodeView.LogicNode.Coordinates == position)
                    .Select(nodeView => nodeView.ID)
                    .SingleOrDefault();
        }
    }
}
