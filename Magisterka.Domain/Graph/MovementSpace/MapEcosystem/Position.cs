using System;
using Magisterka.Domain.Graph.MovementSpace.Exceptions;

namespace Magisterka.Domain.Graph.MovementSpace.MapEcosystem
{
    public class Position : IComparable
    {
        public int? X { get; set; }
        public int? Y { get; set; }

        private readonly Guid _nodeId;

        public Position(Guid nodeId)
        {
            _nodeId = nodeId;
        }

        public int CompareTo(object obj)
        {
            var anotherPosition = obj as Position;

            return _nodeId.CompareTo(anotherPosition?._nodeId);
        }

        public override bool Equals(object anotherNodePosition)
        {
            return anotherNodePosition is Position && ((Position)anotherNodePosition)._nodeId == _nodeId;
        }

        public override int GetHashCode()
        {
            return _nodeId.GetHashCode();
        }

        public static bool operator ==(Position a, Position b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Position a, Position b)
        {
            return !a.Equals(b);
        }

        public int ManhattanDistanceTo(Position anotherPosition)
        {
            return Math.Abs(X.Value - anotherPosition.X.Value) + Math.Abs(Y.Value - anotherPosition.Y.Value);
        }
    }
}
