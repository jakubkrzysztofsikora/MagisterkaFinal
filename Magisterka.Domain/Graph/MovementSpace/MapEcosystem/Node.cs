using System;
using System.Collections.Generic;

namespace Magisterka.Domain.Graph.MovementSpace.MapEcosystem
{
    public class Node
    {
        public Position Coordinates { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsStartingNode { get; set; }
        public bool IsTargetNode { get; set; }

        public IDictionary<Node, EdgeCost> Neighbors { get; set; }

        public Node()
        {
            Coordinates = new Position(Guid.NewGuid());
            Neighbors = new Dictionary<Node, EdgeCost>();
        }

        public Node(IDictionary<Node, EdgeCost> neighbors)
        {
            Coordinates = new Position(Guid.NewGuid());
            Neighbors = neighbors;
        }

        public bool IsNeighborWith(Node nodeToCheck)
        {
            return Neighbors.ContainsKey(nodeToCheck);
        }

        public override bool Equals(object anotherNode)
        {
            return anotherNode is Node && ((Node)anotherNode).Coordinates == Coordinates;
        }

        public override int GetHashCode()
        {
            return Coordinates.GetHashCode();
        }

        public static bool operator ==(Node a, Node b)
        {
            if (ReferenceEquals(a, null))
            {
                return ReferenceEquals(b, null);
            }

            return a.Equals(b);
        }

        public static bool operator !=(Node a, Node b)
        {
            if (ReferenceEquals(a, null))
            {
                return !ReferenceEquals(b, null);
            }

            return !a.Equals(b);
        }
    }
}
