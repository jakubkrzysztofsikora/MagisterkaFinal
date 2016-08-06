using System;
using System.Collections.Generic;

namespace Magisterka.Domain.Graph.MovementSpace.MapEcosystem
{
    public class Edge
    {
        private readonly Guid _id;

        public Edge()
        {
            _id = Guid.NewGuid();
        }

        public int Cost { get; set; }
        public KeyValuePair<Node, Node> NodesConnected { get; set; }

        public override bool Equals(object anotherEdge)
        {
            return anotherEdge is Edge && ((Edge)anotherEdge)._id == _id;
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        public static bool operator ==(Edge a, Edge b)
        {
            if (ReferenceEquals(a, null))
            {
                return ReferenceEquals(b, null);
            }

            return a.Equals(b);
        }

        public static bool operator !=(Edge a, Edge b)
        {
            if (ReferenceEquals(a, null))
            {
                return !ReferenceEquals(b, null);
            }

            return !a.Equals(b);
        }
    }
}
