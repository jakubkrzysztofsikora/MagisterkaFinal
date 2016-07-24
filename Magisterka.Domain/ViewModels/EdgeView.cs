using System.ComponentModel;
using System.Xml.Serialization;
using GraphX.PCL.Common.Models;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;

namespace Magisterka.Domain.ViewModels
{
    public class EdgeView : EdgeBase<NodeView>
    {
        public EdgeView(Edge logicEdge, NodeView source, NodeView destination) : base(source, destination, logicEdge.Cost)
        {
            LogicEdge = logicEdge;
        }

        public Edge LogicEdge { get; set; }

        [XmlAttribute("text")]
        [DefaultValue("Edge")]
        public string Caption { get; set; }

        public override string ToString()
        {
            return Caption;
        }
    }
}
