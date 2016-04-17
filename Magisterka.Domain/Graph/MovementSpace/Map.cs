using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;

namespace Magisterka.Domain.Graph.MovementSpace
{
    public class Map : ICollection<Node>
    {
        public int MaximumNumberOfNodes { get; }

        private IEnumerable<Node> _nodes;

        public Map(int maxNumberOfNodes)
        {
            _nodes = new List<Node>();
            MaximumNumberOfNodes = maxNumberOfNodes;
        }

        public Map(IEnumerable<Node> nodes)
        {
            var nodeList = nodes as IList<Node> ?? nodes.ToList();
            _nodes = nodeList;
            MaximumNumberOfNodes = nodeList.Count;
        }

        public Node GetNodeByPosition(Position position)
        {
            return _nodes.FirstOrDefault(node => node.Coordinates == position);
        }

        public IEnumerable<EdgeCost> GetAllEdgeCosts()
        {
            return from node in _nodes from neighbor in node.Neighbors select neighbor.Value;
        }

        public IEnumerator<Node> GetEnumerator()
        {
            return _nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(Node item)
        {
            if (Count >= MaximumNumberOfNodes)
                throw new IndexOutOfRangeException();

            _nodes = _nodes.Concat(new[] { item });
        }

        public void AddIfNotExists(Node item)
        {
            if (!_nodes.Contains(item))
                Add(item);
        }

        public void Clear()
        {
            _nodes = new List<Node>();
        }

        public bool Contains(Node item)
        {
            return _nodes.Contains(item);
        }

        public void CopyTo(Node[] array, int arrayIndex)
        {
            for (int i = arrayIndex; i < _nodes.Count(); ++i)
            {
                array[i] = _nodes.ElementAtOrDefault(i);
            }
        }

        public bool Remove(Node item)
        {
            _nodes = _nodes.Where(node => node != item);

            return _nodes.Contains(item);
        }

        public Map DeepCopy()
        {
            Map newMap = new Map(new List<Node>(_nodes));
            return newMap;
        }

        public int Count => _nodes.Count();

        public bool IsReadOnly => true;
    }
}
