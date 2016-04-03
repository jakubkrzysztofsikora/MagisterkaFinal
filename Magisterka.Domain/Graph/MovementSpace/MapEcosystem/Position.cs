using System;

namespace Magisterka.Domain.Graph.MovementSpace.MapEcosystem
{
    public class Position : IComparable
    {
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
    }
}
