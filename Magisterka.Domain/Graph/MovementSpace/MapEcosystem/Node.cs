﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using GraphX.PCL.Common.Models;

namespace Magisterka.Domain.Graph.MovementSpace.MapEcosystem
{
    public class Node
    {
        public Position Coordinates { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsStartingNode { get; set; }
        public bool IsTargetNode { get; set; }
        public string Name { get; set; }

        public IDictionary<Node, Edge> Neighbors { get; set; }

        public Node()
        {
            Coordinates = new Position(Guid.NewGuid());
            Neighbors = new Dictionary<Node, Edge>();
            Name = Guid.NewGuid().ToString("n");
        }

        public Node(IDictionary<Node, Edge> neighbors)
        {
            Coordinates = new Position(Guid.NewGuid());
            Neighbors = neighbors;
        }

        public bool IsNeighborWith(Node nodeToCheck)
        {
            return Neighbors.ContainsKey(nodeToCheck);
        }

        public bool IsOnTheGrid()
        {
            return Coordinates.X.HasValue && Coordinates.Y.HasValue;
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
