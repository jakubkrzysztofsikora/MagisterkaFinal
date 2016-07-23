using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GraphX.PCL.Common.Models;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;
using QuickGraph;

namespace Magisterka.Domain.ViewModels
{
    public class EdgeView : EdgeBase<NodeView>
    {
        public Edge LogicEdge { get; set; }

        public EdgeView(Edge logicEdge, NodeView source, NodeView destination) : base(source, destination, logicEdge.Cost)
        {
            LogicEdge = logicEdge;
        }

        [XmlAttribute("text")]
        [DefaultValue("Edge")]
        public string Caption { get; set; }

        public override string ToString()
        {
            return Caption;
        }
    }
}
