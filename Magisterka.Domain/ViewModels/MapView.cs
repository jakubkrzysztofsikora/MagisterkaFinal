using System.Linq;
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

        public void ChangeEdgeCost(EdgeView edge, int newCost)
        {
            edge.Weight = newCost;
            edge.SetStandardCaption();
        }
    }
}
