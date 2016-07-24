using GraphX.PCL.Common.Models;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;

namespace Magisterka.Domain.ViewModels
{
    public class NodeView : VertexBase
    {
        public NodeView()
        {
            CurrentState = eVertexState.Other;
        }

        public Node LogicNode { get; set; }

        public string Caption { get; set; }

        public string Text { get; set; }

        public eVertexState CurrentState { get; set; }

        public override string ToString()
        {
            return $"{Text} {Caption}";
        }
    }
}
