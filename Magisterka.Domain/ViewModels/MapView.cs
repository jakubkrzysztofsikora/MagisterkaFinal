using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;
using QuickGraph;

namespace Magisterka.Domain.ViewModels
{
    public class MapView : IMutableBidirectionalGraph<NodeView, EdgeView>
    {
        public bool IsDirected => false;

        public bool IsVerticesEmpty => !Vertices.Any();

        public int VertexCount => Vertices.Count();

        public IEnumerable<NodeView> Vertices { get; }
        public bool AllowParallelEdges { get; }

        
        public bool ContainsVertex(NodeView vertex)
        {
            throw new NotImplementedException();
        }

        public bool IsOutEdgesEmpty(NodeView v)
        {
            throw new NotImplementedException();
        }

        public int OutDegree(NodeView v)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EdgeView> OutEdges(NodeView v)
        {
            throw new NotImplementedException();
        }

        public bool TryGetOutEdges(NodeView v, out IEnumerable<EdgeView> edges)
        {
            throw new NotImplementedException();
        }

        public EdgeView OutEdge(NodeView v, int index)
        {
            throw new NotImplementedException();
        }

        public bool ContainsEdge(NodeView source, NodeView target)
        {
            throw new NotImplementedException();
        }

        public bool TryGetEdges(NodeView source, NodeView target, out IEnumerable<EdgeView> edges)
        {
            throw new NotImplementedException();
        }

        public bool TryGetEdge(NodeView source, NodeView target, out EdgeView edge)
        {
            throw new NotImplementedException();
        }

        
        public bool ContainsEdge(EdgeView edge)
        {
            throw new NotImplementedException();
        }

        public bool IsEdgesEmpty { get; }
        public int EdgeCount { get; }
        public IEnumerable<EdgeView> Edges { get; }
        public bool IsInEdgesEmpty(NodeView v)
        {
            throw new NotImplementedException();
        }

        public int InDegree(NodeView v)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EdgeView> InEdges(NodeView v)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInEdges(NodeView v, out IEnumerable<EdgeView> edges)
        {
            throw new NotImplementedException();
        }

        public EdgeView InEdge(NodeView v, int index)
        {
            throw new NotImplementedException();
        }

        public int Degree(NodeView v)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public event EventHandler Cleared;
        public int RemoveOutEdgeIf(NodeView v, EdgePredicate<NodeView, EdgeView> predicate)
        {
            throw new NotImplementedException();
        }

        public void ClearOutEdges(NodeView v)
        {
            throw new NotImplementedException();
        }

        public void TrimEdgeExcess()
        {
            throw new NotImplementedException();
        }

        public bool AddVertex(NodeView v)
        {
            throw new NotImplementedException();
        }

        public int AddVertexRange(IEnumerable<NodeView> vertices)
        {
            throw new NotImplementedException();
        }

        public bool RemoveVertex(NodeView v)
        {
            throw new NotImplementedException();
        }

        public int RemoveVertexIf(VertexPredicate<NodeView> pred)
        {
            throw new NotImplementedException();
        }

        public event VertexAction<NodeView> VertexAdded;
        public event VertexAction<NodeView> VertexRemoved;
        public bool AddEdge(EdgeView edge)
        {
            throw new NotImplementedException();
        }

        public int AddEdgeRange(IEnumerable<EdgeView> edges)
        {
            throw new NotImplementedException();
        }

        public bool RemoveEdge(EdgeView edge)
        {
            throw new NotImplementedException();
        }

        public int RemoveEdgeIf(EdgePredicate<NodeView, EdgeView> predicate)
        {
            throw new NotImplementedException();
        }

        public event EdgeAction<NodeView, EdgeView> EdgeAdded;
        public event EdgeAction<NodeView, EdgeView> EdgeRemoved;
        public bool AddVerticesAndEdge(EdgeView edge)
        {
            throw new NotImplementedException();
        }

        public int AddVerticesAndEdgeRange(IEnumerable<EdgeView> edges)
        {
            throw new NotImplementedException();
        }

        public int RemoveInEdgeIf(NodeView v, EdgePredicate<NodeView, EdgeView> edgePredicate)
        {
            throw new NotImplementedException();
        }

        public void ClearInEdges(NodeView v)
        {
            throw new NotImplementedException();
        }

        public void ClearEdges(NodeView v)
        {
            throw new NotImplementedException();
        }
    }
}
