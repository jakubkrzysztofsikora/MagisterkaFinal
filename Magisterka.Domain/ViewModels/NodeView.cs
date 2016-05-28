using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GraphX.PCL.Common.Models;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;

namespace Magisterka.Domain.ViewModels
{
    public class NodeView : VertexBase
    {
        public Node LogicNode { get; set; }

        [XmlAttribute("text")]
        [DefaultValue("Vertex")]
        public string Caption { get; set; }

        public override string ToString()
        {
            return Caption;
        }
    }
}
